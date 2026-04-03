using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;

namespace carkaashiv_angular_API.Services
{
    public class CartService :ICartService 
    {
      
        private readonly AppDbContext _context; 

        public CartService(AppDbContext context )        {
           
            _context = context;
        }

        public async Task<string> AddToCartAsync(int userId, AddToCartRequestDto request)
        {
      var existingItem = await _context.tbl_cart.FirstOrDefaultAsync(
            c=> c.UId == userId &&
                c.PartID == request.PartId);

            if(existingItem != null)
            {
                //update quantity
                existingItem.Quantity += request.Quantity;
                existingItem.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return "Item updated successfully";
            }
            else
            {
                //add part
                var cartItem = new Cart
                {
                    UId = userId,
                    PartID = request.PartId,
                    Quantity = request.Quantity
                };
                _context.tbl_cart.Add(cartItem);
                await _context.SaveChangesAsync();
                return "Item added to cart successfully";
            }           
         
        }
        public async Task<List<CartItemResponseDto>> GetCartItemsAsync(int userId)
        {
            return await _context.tbl_cart.Where(c => c.UId == userId).
                    Join(_context.tbl_part,
                    cart => cart.PartID,
                    part => part.PartId,
                   (cart, part) => new CartItemResponseDto
                   {
                       CartId = cart.CartId,
                       PartId = cart.PartID,
                       PartName = part.PName,
                       Price = part.PPrice ?? 0,
                       Quantity = cart.Quantity,
                       SubTotal = (part.PPrice ?? 0) * cart.Quantity,
                       ImageUrl = part.ImagePath
                   }).ToListAsync();
        }
        public async Task<string> UpdateCartQuantityAsync(int userId ,UpdateCartQuantityRequestDto request)
        {
            var item = await _context.tbl_cart.FirstOrDefaultAsync(
                c => c.UId == userId && 
                     c.PartID == request.PartId
                );
            if(item == null) throw new Exception("Cart item not found");

            item.Quantity = request.Quantity;
            item.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return "Cart quantity update successfully";
        }
       
       public async Task<string> RemoveCartItemAsync(int userId,int partId )
        {
            var item = await _context.tbl_cart.FirstOrDefaultAsync(
                c => c.UId == userId &&
                    c.PartID == partId);
            if (item == null) throw new Exception("Cart item not found");
            _context.tbl_cart.Remove(item);
            await _context.SaveChangesAsync();
            return "Item removed from cart sucessfully";
        }
        public async Task<int> GetCartCountAsync(int userId)
        {
            return await _context.tbl_cart
               .Where(c => c.UId == userId).SumAsync(c => c.Quantity);// SumAsync gives total quantity count, which is ideal for badge.
        }
    }
}
