namespace carkaashiv_angular_API.DTOs
{
    public class PlaceOrderRequest
    {
        public string DeliveryName { get; set; } = string.Empty;

        public string DeliveryPhone { get; set; } = string.Empty;

        public string DeliveryAddress { get; set; } = string.Empty;

        public string? Landmark { get; set; }

    }
}
