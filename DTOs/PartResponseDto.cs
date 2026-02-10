
namespace carkaashiv_angular_API.DTOs
{
    public class PartResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
       /** DB still uses ImagePath, API can expose ImageUrl**/
    }
}
