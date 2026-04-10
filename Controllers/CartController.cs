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
    [Authorize(Roles = "customer")] // cart is exclusive of customer only
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : BaseController
    {

        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

            [HttpPost("add")]
            public async Task<IActionResult> AddToCart(AddToCartRequestDto request)
            {          
            var message =  await _cartService.AddToCartAsync(CurrentUserId, request);
            return Ok(ApiResponse<string>.Ok(message));
        }

      
        
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var items = await _cartService.GetCartItemsAsync(CurrentUserId);
            var message = items.Any() ? "Cart fetched Succesfully" : "Cart is empty";
            return Ok(ApiResponse<List<CartItemResponseDto>>.Ok( message, items) );             

        }

       
        [HttpPut("update-quantity")]
        public async Task<IActionResult>updateQuanity(UpdateCartQuantityRequestDto request)
        {
            var message = await _cartService.UpdateCartQuantityAsync(CurrentUserId, request);
            return Ok(ApiResponse<string>.Ok(message));
        }


       
        [HttpDelete("remove/{partId}")]
        public async Task<IActionResult> RemoveItem(int partId)
        {
            var message = await _cartService.RemoveCartItemAsync(CurrentUserId, partId);
            return Ok(ApiResponse<string>.Ok(message));
        }

        
        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount()
        {
            var count = await _cartService.GetCartCountAsync(CurrentUserId);
            return Ok(ApiResponse<int>.Ok("Cart count fetched successfully",count));

        }
    } 

}