
using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models.Auth;
using carkaashiv_angular_API.Models.Shared;
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
           
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;


        public AuthController( IAuthService authService,ITokenService tokenService)
        {
            
            _authService = authService;
            _tokenService = tokenService; 
        }
  
        //======Customer(User) Registration Flow=======

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser(RegisterUserDto dto)
        {
            var success = await _authService.RegisterUserAsync(dto);
            if (!success)
                return Conflict(ApiResponse<object>.Fail("Mobile number already registered."));


            return Ok(ApiResponse<object>.Ok("User Registered sucessfully"));
        }


        //======Employee Registration flow=======

        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee(RegisterEmployeeDto dto)
        {

            var success = await _authService.RegisterEmployeeAsync(dto);
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
            //Generate token
            var token = _tokenService.GenerateJwtToken(
                result.Data!.Id,
                result.Data.Role!,
                result.Data.Email!
               );
            //Set jwt cookies
            _tokenService.SetJwtCookie(token);

            return Ok(ApiResponse<LoginResponseDto>.Ok(
                result.Message,
                result.Data));
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            //** Delete cookie by name
            _tokenService.ClearJwtCookie(Response);         
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

    }
}