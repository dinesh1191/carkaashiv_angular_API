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
        public string? DeliveryName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool  CanEditAddress { get; set; }
    }
}
