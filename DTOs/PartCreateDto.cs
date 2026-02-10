using System.ComponentModel.DataAnnotations;

namespace carkaashiv_angular_API.DTOs
{
    public class PartCreateDto
    {

        [Required(ErrorMessage ="Employee Id is required")]
        public int EmployeeId { get; set; }
        [MinLength(3,ErrorMessage ="Parts name must be at least 3 characters")]
        [MaxLength(25,ErrorMessage ="Parts name cannot exceed 25 characters")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        [Range(0.01,double.MaxValue,ErrorMessage ="Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="Parts stock is required")]
        [Range(0, int.MaxValue,ErrorMessage ="Stock cannot be negative")]
        public int Stock {  get; set; }

    }
}
