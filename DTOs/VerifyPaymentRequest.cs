namespace carkaashiv_angular_API.DTOs
{
    public class VerifyPaymentRequest
    {
        public decimal VerifiedAmount { get; set; } // actual recieved amount
        public string? Status { get; set; } // Verified/Rejected
        public string? Remarks { get; set; }

    }
}
