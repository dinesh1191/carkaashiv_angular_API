using System.ComponentModel.DataAnnotations;

namespace carkaashiv_angular_API.DTOs
{
    public class PartUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0.01,double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        [Range(0,int.MaxValue)]
        public int stock { get; set; }
    }
}
