using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Enums;

namespace carkaashiv_angular_API.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> PlaceOrderAsync(int currentUserId, string idempotencyKey);
        Task<OrderDetailDto> GetOrderByIdAsync(int currentUserId, int orderId);
        Task <SubmitPaymentResult>SubmitPaymentAsync(int currentUserId, int orderId, SubmitPaymentRequest request);
        Task<List<AdminOrderDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<VerifyPaymentResult> VerifyPaymentAsync(int orderId);
        Task MarkAsShippedAsync(int orderId);
        Task<List<MyOrderDto>> GetMyOrdersAsync(int currentUserId);
    }
}
