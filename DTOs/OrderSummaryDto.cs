namespace carkaashiv_angular_API.DTOs
{
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
