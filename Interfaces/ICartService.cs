using carkaashiv_angular_API.DTOs;

namespace carkaashiv_angular_API.Interfaces
{
    public interface ICartService
    {
        Task <string> AddToCartAsync(int userId, AddToCartRequestDto request);
        Task<List<CartItemResponseDto>> GetCartItemsAsync(int userId);
    }
}
