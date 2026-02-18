using BCrypt;
using BCrypt.Net;
using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Auth;
using carkaashiv_angular_API.Services;
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
using static carkaashiv_angular_API.Services.AuthService;

namespace carkaashiv_angular_API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly IAuthService _service;
        private readonly AuthService _authService;
     

        public AuthController(IConfiguration config, AppDbContext context, IAuthService service, AuthService AuthService)
        {
            _config = config;
            _context = context;
            _service = service;
            _authService = AuthService;

        }
        [HttpGet("db-test")]
        public async Task<IActionResult> TestDb()
        {
            try
            {
                await _context.Database.OpenConnectionAsync();
                return Ok("Database connected");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
            {
                // CLEAR EXISTING COOKIE if any
                Response.Cookies.Delete("jwtToken");
                return Unauthorized(ApiResponse<string>.Fail(result.Message));
            }
            var token = GenerateJwtToken(
                result.Data!.Id,
                result.Data.Role!,
                result.Data.Email!
               );

            SetJwtCookie(token);

            return Ok(ApiResponse<LoginResponseDto>.Ok(
                result.Message,
                result.Data));
        }



        //if (!ModelState.IsValid)
        //    return BadRequest(ApiResponse<string>.Fail("Invalid input"));
        //try
        //{
        //    //check if input is looks like an email (Employee) or phone(user/customer)
        //    bool isEmail = request.Username.Contains("@");

        //    string dummyHash = "$2a$11$C6UzMDM.H6dfI/f/IKcEe.6o9pJp2sS1Xg1JxE0hH6E3D1e9wXy6e";

        //    if (isEmail)
        //    {
        //        var employee = await _context.tbl_emp

        //            .FirstOrDefaultAsync(e => e.Email == request.Username);
        //        if (employee == null)
        //        {
        //            BCrypt.Net.BCrypt.Verify(request.Password, dummyHash);
        //            return Unauthorized(ApiResponse<string>.Fail("Invalid Credentials"));

        //        }
        //        if (!BCrypt.Net.BCrypt.Verify(request.Password, employee.EmpPasswordHash))
        //            return Unauthorized(ApiResponse<String>.Fail("Invalid credentials"));
        //        if (!employee.IsActive)
        //        {
        //            return Unauthorized(ApiResponse<string>.Fail("Account disabled"));
        //        }
        //        return GenerateLoginResponse(employee.Id,/*** employee.Name,**/employee.Email, employee.Role, 
        //            new
        //            {   employee.Id,
        //                employee.Name,
        //                employee.Email,
        //                employee.Role,
        //            });
        //    }                            

        //    else
        //    //======Customer login flow=======
        //    {
        //        // Find user/customer 
        //        var customer = await _context.tbl_user.FirstOrDefaultAsync(c => c.Phone == request.Username);
        //        {
        //            if (customer == null)
        //            {
        //                return Unauthorized(ApiResponse<string>.Fail("User not found"));
        //            }

        //        }
        //        // Validate Password
        //        if (!BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
        //        {
        //            return Unauthorized(ApiResponse<string>.Fail("Invalid Password"));
        //        }

        //        // Generate JWT with userId,name and role
        //        var token = GenerateJwtToken(customer.Id, customer.Name!, "customer"); //explicitly passing role as customer
        //        SetJwtCookie(token);
        //        return Ok(ApiResponse<object>.Ok("Customer Login Successful",
        //            new
        //            {
        //                customer.Phone,
        //                customer.Role,
        //                customer.Id
        //            }));
        //    }
        //}
        //catch (Exception ex)
        //{
        //    return StatusCode(500, ApiResponse<string>.Fail($"Server error:{ex.Message}"));
        //}

   


        private IActionResult GenerateLoginResponse(int userId, string name, string role, object responseData)
        {
            var token = GenerateJwtToken(userId, name, role);

            SetJwtCookie(token);

            return Ok(ApiResponse<LoginResponse>.Ok("Login successful"));
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(int userId, string nameOrEmail, string role)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId",userId.ToString()),
                new Claim(ClaimTypes.Role,role),
                new Claim(ClaimTypes.Name,nameOrEmail)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),// token valid for one hour
                signingCredentials: creds
                );
            Console.WriteLine("JWT CREATE Issuer: " + jwtSettings["Issuer"]);
            Console.WriteLine("JWT CREATE Audience: " + jwtSettings["Audience"]);
            Console.WriteLine("JWT CREATE Key Len: " + jwtSettings["Key"]?.Length);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //Helper method for cookie setup handle both prod and dev environment        
        private void SetJwtCookie(string token)
        {
            var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "PRODUCTION";
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,               
                Secure = true,   // Required for SameSite=None (local + prod)              
                SameSite = SameSiteMode.None,    // Angular SPA → API (cross-site)XHR
                Expires = DateTime.UtcNow.AddMinutes(isProduction ? 20 : 30), //prod token expires faster
                Path = "/"
            };
            Response.Cookies.Append("jwtToken", token, cookieOptions);

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
        public async Task<ActionResult<object>> Me()
        {


            try
            {
                // Extract user info from token              
                var userClaimId = User.Claims.FirstOrDefault(C => C.Type == "userId")?.Value;
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (userClaimId == null)
                {
                    return Unauthorized(ApiResponse<object>.Fail("Unauthorised"));

                }
                object? userData = null;

                if (_context == null)
                    throw new Exception("DB context is not injected properly!");
                if (role == "customer")
                {
                    userData = await _context.tbl_user
                        .Where(c => c.Id == int.Parse(userClaimId))
                        .Select(c => new { c.Id, c.Name, c.Email, c.Role })
                        .FirstOrDefaultAsync();
                }
                else // staff or admin
                {
                    userData = await _context.tbl_emp
                .Where(e => e.Id == int.Parse(userClaimId))
                .Select(e => new { e.Id, e.Name, e.Email, e.Role })
                .FirstOrDefaultAsync();

                }
                if (userData == null)
                    return NotFound(ApiResponse<object>.Fail("User not found"));

                return Ok(ApiResponse<object>.Ok("User fetched successfully", userData));
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }


        //======Customer(User) Registration Flow=======

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser(RegisterUserDto dto)
        {
            var success = await _service.RegisterUserAsync(dto);
            if (!success)
                return Conflict(ApiResponse<object>.Fail("Mobile number already registered."));
      
           
            return Ok(ApiResponse<object>.Ok("User Registered sucessfully"));
        }


        //======Employee Registration flow=======

        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee(RegisterEmployeeDto dto)
        {

            var success = await _service.RegisterEmployeeAsync(dto);
            if (!success)
                return Conflict(ApiResponse<object>.Fail("Email already Exists!"));

            return Ok(ApiResponse<object>.Ok("Employee registered successfully"));

        }

    }
}