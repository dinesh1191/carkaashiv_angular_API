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
        Task<VerifyPaymentResult> VerifyPaymentAsync(int orderId, VerifyPaymentRequest request);
        //Task<List<OrderSummaryDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<PaymentReviewQueueDto>> GetPaymentReviewQueueAsync();


    }
}
