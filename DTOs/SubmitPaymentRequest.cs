namespace carkaashiv_angular_API.DTOs
{
    public class SubmitPaymentRequest
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentReference { get; set; } = string.Empty;
        public string TempKey { get; set; } = string.Empty;
    }
}
