using carkaashiv_angular_API.Models.Enums;

namespace carkaashiv_angular_API.DTOs
{
    public class MyOrderDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? OrderStatusText {  get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PaymentProofUrl { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? RecipientAddress { get; set; }
        public string? LandMark { get; set; }
        public bool  CanEditAddress { get; set; }
    }
}
