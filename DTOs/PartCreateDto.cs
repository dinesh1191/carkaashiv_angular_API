using System.ComponentModel.DataAnnotations;

namespace carkaashiv_angular_API.DTOs
{
    public class PartCreateDto
   {     
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Part description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="Parts stock is required")]
        [Range(0, int.MaxValue,ErrorMessage ="Stock cannot be negative")]
        public int Stock {  get; set; }
        
        [Required]
        [StringLength(500)]
        public string? ImageKey { get; set; } 

    }
}
