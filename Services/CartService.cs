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
    }
}
