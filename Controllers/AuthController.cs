using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Auth;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
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
        public AuthController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "dini@kaashiv.com" && request.Password == "$2a$11$5Gg3g.AQ7.j08J.sM6O3QOckGT1wBv9Z9nyCz86.HoVoWNjxaAskO")
            {
                // Create secure HttpOnly cookie
                var token = GenerateJwtToken(request.Username, "Admin");

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, //use true only in production (HTTPS)required for SameSite=None
                    SameSite = SameSiteMode.None, //crucial for cross - origin(Angular <-> .NET)
                    Expires = DateTime.UtcNow.AddHours(1)
                };
                // Attach token to cookie
                Response.Cookies.Append("jwtToken", token, cookieOptions);

                return Ok(new ApiResponse<LoginResponse>(
                    true,
                    "Login Successful",
                    new LoginResponse
                    {
                        Username = request.Username,
                        Role = "Admin"
                    }
                    ));
            }
            return Unauthorized(new ApiResponse<string>
            (
               false,
               "Invalid Username or password"
            ));
        }


        // Helper method to generate JWT token

        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.Role,role)
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            //*** Clear the cookie by setting an expired cookie
            Response.Cookies.Append("jwtToken","",new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1) // expired cookie
            });

            return Ok(new ApiResponse<string>
                (
                true,
                "Login Successful,jwt Cleared"
                ));
        }





    }


 
    
    
    
    }