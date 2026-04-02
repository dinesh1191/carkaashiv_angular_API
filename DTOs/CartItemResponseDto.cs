namespace carkaashiv_angular_API.DTOs
{
    public class CartItemResponseDto
    {

        public int CartId { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public string? ImageUrl { get; set; }


    }
}
