using carkaashiv_angular_API.Models;

namespace carkaashiv_angular_API.DTOs
{
    public class AddToCartRequestDto
    {     
        public int UId { get; set; }
        public int PartId { get; set; }
        public int Quantity { get; set; }    
            
    }
}
