using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt;
using BCrypt.Net;

namespace carkaashiv_angular_API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        public AuthController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try{
                // Find employee by email 
                var employee = await _context.tbl_emp.FirstOrDefaultAsync(e => e.Email == request.Username);
                if(employee == null)
                {
                    return Unauthorized(ApiResponse<string>.Fail("User not found"));
                }
                // Validate password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, employee.EmpPasswordHash))
                {
                    return Unauthorized(ApiResponse<string>.Fail("Invalid password"));

                }
                // Generate JWT with userId,name and role
                var token = GenerateJwtToken(employee);

                //Create cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, //use true only in production (HTTPS)required for SameSite=None
                    SameSite = SameSiteMode.None, //crucial for cross - origin(Angular <-> .NET)
                    Expires = DateTime.UtcNow.AddHours(1)

                };
                Response.Cookies.Append("jwtToken", token, cookieOptions);
                //Return success
                return Ok(ApiResponse<object>.Ok("Login Successful",new
                {
                    employee.Email,
                    employee.Role,
                    employee.Id
                }));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail($"Server error:{ex.Message}"));
            }

           
        }


        // Helper method to generate JWT token

        private string GenerateJwtToken(TableEmployee employee)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", employee.Id.ToString()),         
                new Claim(ClaimTypes.Role,employee.Role!),// ! explicitly setting field will not be null
                new Claim(ClaimTypes.Name,employee.Name!)
                
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),// token valid for one hour
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            //** Delete cookie by name
            Response.Cookies.Delete("jwtToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,              
            });          
            return Ok(ApiResponse<string>.Ok("Logout successful. JWT cleared."));
             }


            [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<TableEmployee>> Me()   
        {
                   

          try
            {
                // Extract user info from token              
                var userClaimId = User.Claims.FirstOrDefault(C => C.Type == "userId")?.Value;
               
                if (userClaimId == null)
                {
                    return Unauthorized(ApiResponse<TableEmployee>.Fail("Unauthorised"));

                }
                // Fetch from DB
                var employee = await _context.tbl_emp.FindAsync(int.Parse(userClaimId));

                if (employee == null)
                {
                    return NotFound(ApiResponse<TableEmployee>.Fail("Id not found"));
                }
                var response = new
                {
                    employee.Name,
                    employee.Id,
                    employee.Email,
                    employee.Role
                };
                return Ok(ApiResponse<object>.Ok("Employee fetched details successfully", response));

            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}