namespace carkaashiv_angular_API.DTOs
{
    public class AdminOrderDetailsDto
    {
        public int OrderId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientPhone { get; set; } = string.Empty;
        public string RecipientAddress { get; set; } = string.Empty;
        public string LandMark { get; set; } = string.Empty; 
        public decimal GstTaxAmount { get; set; }        
        public decimal TotalAmount { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public List<AdminOrderItemDto> Items { get; set; } = [];

    }
}
