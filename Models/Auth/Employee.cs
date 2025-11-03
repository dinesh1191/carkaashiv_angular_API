namespace carkaashiv_angular_API.Models.Auth
{
    public class Employee
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set;  } = "employee";

    }
}
