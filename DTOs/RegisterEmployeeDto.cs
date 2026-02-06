using Microsoft.AspNetCore.Identity;

namespace carkaashiv_angular_API.DTOs
{
    public class RegisterEmployeeDto
    {

        public string Name { get; set; } = string.Empty;
        public string Role {  get; set; }  = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
