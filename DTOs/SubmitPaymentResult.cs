namespace carkaashiv_angular_API.DTOs
{
    public class SubmitPaymentResult
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
