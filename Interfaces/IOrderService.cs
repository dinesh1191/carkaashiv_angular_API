using carkaashiv_angular_API.DTOs;

namespace carkaashiv_angular_API.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto>PlaceOrderAsync(int currentUserId, string idempotencyKey);
        Task<OrderDetailDto>GetOrderByIdAsync(int currentUserId, int orderId);
    }
}
