namespace carkaashiv_angular_API.DTOs
{
    public class PaymentReviewQueueDto
    {
        public int OrderId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string? PaymentProofUrl { get; set; }

        public string? PaymentReference { get; set; }

        public DateTime? SubmittedAt { get; set; }
    }
}
