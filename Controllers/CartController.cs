using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Shared;
using carkaashiv_angular_API.Services;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace carkaashiv_angular_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {

        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

            [Authorize]
            [HttpPost("add")]
            public async Task<IActionResult> AddToCart(AddToCartRequestDto request)
            {
                var userId = GetUserIdFromToken(User);
            Console.WriteLine("check userId: "+userId, GetUserIdFromToken(User));

              var message =  await _cartService.AddToCartAsync(userId, request);
            return Ok(new
            {
              message
            });
            }

      
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserIdFromToken(User);
            var items = await _cartService.GetCartItemsAsync(userId);
            var message = items.Any() ? "Cart fetched Succesfully" : "Cart is empty";
            return Ok(ApiResponse<List<CartItemResponseDto>>.Ok(
              message,items));             

        }
        [Authorize]
        [HttpPut("update-quantity")]
        public async Task<IActionResult>updateQuanity(UpdateCartQuantityRequestDto request)
        {
            var userId = GetUserIdFromToken(User);
            var message = await _cartService.UpdateCartQuantityAsync(userId, request);
            return Ok(ApiResponse<string>.Ok(message));
        }
        [Authorize]
        [HttpDelete("remove/{partId}")]
        public async Task<IActionResult> RemoveItem(int partId)
        {
            var userId = GetUserIdFromToken(User);
            var message = await _cartService.RemoveCartItemAsync(userId, partId);
            return Ok(ApiResponse<string>.Ok(message));
        }     
        
        [NonAction] //Do not treat this as an API action
        public int GetUserIdFromToken(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            Console.WriteLine("getuser id form claim" + userIdClaim);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token: userId missing");

            return int.Parse(userIdClaim);
        }
       
    } 

}