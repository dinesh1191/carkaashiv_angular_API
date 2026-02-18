using System.ComponentModel.DataAnnotations;

namespace carkaashiv_angular_API.DTOs
{
    public class EmployeeAdminUpdateDto
    {
        public string? Name { get; set; } 
        public string? Role { get; set; }
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@kaashiv\.com$",
        ErrorMessage = "Email must be a @kaashiv.com address")]
        public string? Email { get; set; } 
        public string? Password { get; set; } 
        public string? Phone { get; set; } 
    }
}
