using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carkaashiv_angular_API.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]    
    public class OrderController(IOrderService orderService) : BaseController
    {
        private readonly IOrderService _orderService = orderService;

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder()
        {
            var key = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest(new { message = "Idempotency-key header is required"});
            var result = await _orderService.PlaceOrderAsync(CurrentUserId,key);           
            return Ok(result);
        }

        [Authorize(Roles = "customer")] // order is exclusive of customer only
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(CurrentUserId,id);
            return Ok(order);
        }

        [Authorize(Roles = "customer")] 
        [HttpPost("{orderId}/submit-payment")]
        public async Task<IActionResult>SubmitPayment(int orderId, [FromBody] SubmitPaymentRequest request)
        {      
           var result = await _orderService.SubmitPaymentAsync(CurrentUserId, orderId, request);
            return Ok(ApiResponse<SubmitPaymentResult>.Ok("Payment submitted successfully",result));          
        }

        [Authorize(Roles = "admin,employee")] 
        [HttpPost("{orderId}/verify-payment")]
        public async Task<IActionResult>VerifyPayment(int orderId)
        {
          var result = await _orderService.VerifyPaymentAsync(orderId);
            return Ok(ApiResponse<VerifyPaymentResult>.Ok("Payment verification completed",
            result));
        }  

        [Authorize(Roles = "admin,employee")] 
        [HttpGet("payment-review-queue")]
        public async Task<IActionResult> GetPaymentReviewQueue()
        {
            var result = await _orderService.GetPaymentReviewQueueAsync();
            return Ok(ApiResponse<List<PaymentReviewQueueDto>>.Ok("Orders fetched successfully", (result)));
        }
    }
}
