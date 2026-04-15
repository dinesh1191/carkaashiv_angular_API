namespace carkaashiv_angular_API.DTOs
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
