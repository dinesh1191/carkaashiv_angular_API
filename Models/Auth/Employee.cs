namespace carkaashiv_angular_API.Models.Auth
{
    public class Employee
    {
        //public int Id { get; set; }
        //public string Email { get; set; } = string.Empty;
        //public string FullName { get; set; } = string.Empty;
        //public string PasswordHash { get; set; } = string.Empty;
        //public string Role { get; set;  } = "employee";

        public int Id { get; set; }
        public string EmployeeCode { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; }=string.Empty;
        public string PasswordHash { get; set; }=string.Empty;
        public string Role {  get; set; }  = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
