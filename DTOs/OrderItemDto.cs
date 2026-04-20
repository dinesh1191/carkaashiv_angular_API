namespace carkaashiv_angular_API.DTOs
{
    public class OrderItemDto
    {
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
