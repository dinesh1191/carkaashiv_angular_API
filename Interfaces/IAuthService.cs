using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Models.Auth;

namespace carkaashiv_angular_API.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(RegisterUserDto dto);
        Task<bool> RegisterEmployeeAsync(RegisterEmployeeDto dto);
        Task<AuthResult> LoginAsync(LoginRequest request);       

    }
}
