using Azure.Core;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Exceptions;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Enums;
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
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            var key = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest(new { message = "Idempotency-key header is required" });
            var result = await _orderService.PlaceOrderAsync(CurrentUserId,request,key);
            return Ok(result);
        }

        [Authorize(Roles = "customer")] // order is exclusive of customer only
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(CurrentUserId, id);
            return Ok(order);
        }

        [Authorize(Roles = "customer")]
        [HttpPost("{orderId}/submit-payment")]
        public async Task<IActionResult> SubmitPayment(int orderId, [FromBody] SubmitPaymentRequest request)
        {
            var result = await _orderService.SubmitPaymentAsync(CurrentUserId, orderId, request);
            return Ok(ApiResponse<SubmitPaymentResult>.Ok("Payment submitted successfully", result));
        }

        [Authorize(Roles = "admin,employee")]
        [HttpPost("{orderId}/verify-payment")]
        public async Task<IActionResult> VerifyPayment(int orderId)
        {
            var result = await _orderService.VerifyPaymentAsync(orderId);
            return Ok(ApiResponse<VerifyPaymentResult>.Ok("Payment verification completed",
            result));
        }

        [Authorize(Roles = "admin,employee")]
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderStatus status)
        {
            var result = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(ApiResponse<List<AdminOrderDto>>.Ok("Orders fetched successfully", (result)));
        }

        [Authorize(Roles = "admin,employee")]
        [HttpPost("{orderId}/mark-shipped")]
        public async Task<IActionResult> MarkAsShipped(int orderId)
        {
            await _orderService.MarkAsShippedAsync(orderId);
            return Ok(ApiResponse<List<AdminOrderDto>>.Ok("Orders marked as Shipped"));
        }

        [Authorize(Roles = "customer")]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetMyOrdersAsync(CurrentUserId);
            if (!orders.Any())
            {
                throw new BusinessException("You have not placed any orders");
            }

            return Ok(ApiResponse<List<MyOrderDto>>.Ok("Orders fetched successfully",orders));        
        }

   
        [Authorize(Roles = "admin,employee")]
        [HttpGet("ordersDetails/{orderId}")]
        public async Task<IActionResult>GetOrderDetails(int orderId)
        {
            var result = await _orderService.GetOrderDetailsAsync(orderId);
            if(result == null)
            {
              return NotFound(ApiResponse<object>.Fail("Order not found"));      
            }
            return Ok(ApiResponse<object>.Ok("Order retrieved successfully", result));
        }

    }
}
