using carkaashiv_angular_API.Interfaces;
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
            var result = await _orderService.PlaceOrderAsync(CurrentUserId);
            return Ok(result);
        }
    }
}
