namespace carkaashiv_angular_API.DTOs
{
    public class MyOrderDto
    {
        public int OrderId { get; set; }

        public decimal TotalAmount { get; set; }

        public int Status { get; set; } 

        public DateTime CreatedAt { get; set; }

        public string? PaymentProofUrl { get; set; }
    }
}
