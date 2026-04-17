using carkaashiv_angular_API.DTOs;

namespace carkaashiv_angular_API.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto>PlaceOrderAsync(int userId,string idempotencyKey);
    }
}
