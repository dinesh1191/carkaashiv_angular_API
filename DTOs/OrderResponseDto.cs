namespace carkaashiv_angular_API.DTOs
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string deliveryName { get; set; } = string.Empty;
        public string deliveryPhone { get; set; } = string.Empty;
        public string deliveryAddress { get; set; } = string.Empty;
        public string landmark { get; set; } = string.Empty;
    }
}
