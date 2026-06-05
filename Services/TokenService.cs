using Azure;
using carkaashiv_angular_API.Interfaces;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace carkaashiv_angular_API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenService> _logger;
        public TokenService(IConfiguration config, ILogger<TokenService> logger, IHttpContextAccessor accessor)
        {
            _config = config;
            _httpContextAccessor = accessor;
            _logger =logger;

        }


        public string GenerateJwtToken(int userId, string role, string nameOrEmail)
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


        public void SetJwtCookie(string token)
        {
            var context = _httpContextAccessor.HttpContext;
            var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,   // Required for SameSite=None (local + prod)              
                SameSite = SameSiteMode.None,    // Angular SPA → API (cross-site)XHR
                Expires = DateTime.UtcNow.AddMinutes(isProduction ? 20 : 30), //prod token expires faster
                Path = "/",

            };
            _logger.LogInformation("Setting JWT cookie. Secure={Secure}, SameSite={SameSite}, Expiry={Expiry}",
             cookieOptions.Secure,
             cookieOptions.SameSite,
             cookieOptions.Expires);

            context?.Response.Cookies.Append("jwtToken", token, cookieOptions);
            _logger.LogInformation("Response headers dini: {Headers}",
            string.Join(", ", context!.Response.Headers.Keys));
            _logger.LogInformation("JWT cookie appended successfully.");
        }
        
        public void ClearJwtCookie(HttpResponse response)
        {
            response.Cookies.Delete("jwtToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
        }

    }
}
 
