using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace carkaashiv_angular_API.DTOs
{
    public class RegisterEmployeeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Role {  get; set; }  = string.Empty;
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@kaashiv\.com$",
        ErrorMessage = "Email must be a @kaashiv.com address")]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
