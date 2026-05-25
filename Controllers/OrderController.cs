using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carkaashiv_angular_API.Controllers
{

    [Authorize(Roles = "customer")] // cart is exclusive of customer only
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder()
        {
            var key = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest(new { message = "Idempotency-key header is required"});
            var result = await _orderService.PlaceOrderAsync(CurrentUserId,key);           
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(CurrentUserId,id);
            return Ok(order);
        }

        [HttpPost("{orderId}/submit-payment")]
        public async Task<IActionResult>SubmitPayment(int orderId, [FromBody] SubmitPaymentRequest request)
        {      
           var result = await _orderService.SubmitPaymentAsync(CurrentUserId, orderId, request);
            return Ok(ApiResponse<SubmitPaymentResult>.Ok("Payment submitted successfully",result));          
        }

        // [Authorize(Roles = "admin,employee")] // approval only by admin/employee
        [HttpPost("{orderId}/verify-payment")]
        public async Task<IActionResult>VerifyPayment(int orderId,VerifyPaymentRequest request)
        {
          var result = await _orderService.VerifyPaymentAsync(orderId, request);
            return Ok(ApiResponse<VerifyPaymentResult>.Ok("Payment verification completed",
            result));
        }  

        // [Authorize(Roles = "admin,employee")] // approval only by admin/employee
        [HttpGet("payment-review-queue")]
        public async Task<IActionResult> GetPaymentReviewQueue()
        {
            var result = await _orderService.GetPaymentReviewQueueAsync();
            return Ok(ApiResponse<List<PaymentReviewQueueDto>>.Ok("Orders fetched successfully", (result)));
        }

    }
}
