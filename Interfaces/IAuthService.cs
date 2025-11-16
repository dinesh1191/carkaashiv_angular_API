using carkaashiv_angular_API.DTOs;

namespace carkaashiv_angular_API.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(RegisterUserDto dto);
        Task<bool> RegisterEmployeeAsync(RegisterEmployeeDto dto);
    }
}
