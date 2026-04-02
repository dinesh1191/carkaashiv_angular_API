using carkaashiv_angular_API.DTOs;

namespace carkaashiv_angular_API.Interfaces
{
    public interface ICartService
    {
        //For POST addToCart
        Task <string> AddToCartAsync(int userId, AddToCartRequestDto request);
        //For Get
        Task<List<CartItemResponseDto>> GetCartItemsAsync(int userId);
        //For update
        Task<string> UpdateCartQuantityAsync(int userId, UpdateCartQuantityRequestDto request);
        // For Delete
        Task<string> RemoveCartItemAsync(int userId, int partId);
    }
}
