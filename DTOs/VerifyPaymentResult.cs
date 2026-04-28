namespace carkaashiv_angular_API.DTOs
{
    public class VerifyPaymentResult
    {
        public string Label { get; set; } = default!; // UNDERPAID / EXACT / OVERPAID
        public decimal ExpectedAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal MismatchAmount { get; set; }

    }
}
