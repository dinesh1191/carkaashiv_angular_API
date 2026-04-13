using System.ComponentModel.DataAnnotations;

namespace carkaashiv_angular_API.DTOs
{
    public class RegisterUserDto
    {
        public string Name { get; set; } = string.Empty;
        [RegularExpression(@"^[6-9]\d{9}$",
        ErrorMessage = "Phone must be a valid 10-digit mobile number")]
        public string phone { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;      
    }
}
