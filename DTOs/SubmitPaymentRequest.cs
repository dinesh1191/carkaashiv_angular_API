namespace carkaashiv_angular_API.DTOs
{
    public class SubmitPaymentRequest
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentReference { get; set; } = null!;
        public string? PaymentProofUrl { get; set; }
    }
}
