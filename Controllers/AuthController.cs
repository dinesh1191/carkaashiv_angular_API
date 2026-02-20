
using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models.Auth;
using carkaashiv_angular_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
            {
                /*** CLEAR EXISTING COOKIE if any ***/
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

            var userData = await _authService.GetCurrentUserAsync(User);
            if (userData == null)
                return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));
            return Ok(ApiResponse<object>.Ok("User fetched successfully", userData));
            
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

    }
}