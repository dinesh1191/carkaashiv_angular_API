namespace carkaashiv_angular_API.Models.Auth
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "customer";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
