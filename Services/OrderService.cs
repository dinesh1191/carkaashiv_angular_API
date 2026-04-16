using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using static Azure.Core.HttpHeader;

namespace carkaashiv_angular_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<OrderResponseDto> PlaceOrderAsync(int userId)
        {
            // checkout lifecycle: Fetch cart → calculate totals → save bill header → save bill items → clear cart -> commit -> return response

            // Step 1: Fetch all cart items for the current user
            // Include Part entity to access product price details
            var cartItems = await _context.tbl_cart
                .Where(c => c.UId == userId)
                .Include(c => c.Part)
                .ToListAsync();

            // Step 2: Validate cart is not empty
            if (!cartItems.Any())
                throw new ArgumentException("Cart is empty");

            // Step 3: Calculate billing values
            // Subtotal = sum of (quantity * price) for each cart item
            var subtotal = cartItems.Sum(x =>
                x.Quantity * (x.Part?.PPrice ?? 0m));

            // Apply 18% tax
            var tax = subtotal * 0.18m;

            // Final payable amount
            var total = subtotal + tax;

            // Step 4: Start DB transaction
            // Ensures both order header and order items are saved together
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Step 5: Create order header (bill summary)
                var order = new Order
                {
                    UserId = userId,
                    SubtotalAmount = subtotal,
                    TaxAmount = tax,
                    TotalAmount = total,
                    Status = "Completed",
                    InvoiceNumber = string.Empty //create order initially 
                    // created_at handled by DB default
                };

                // Save order first to generate OrderId from DB
                _context.tbl_orders.Add(order);
                await _context.SaveChangesAsync();
                


                // Step 5.1 Generate and presist invoice number using generated OrderId
                order.InvoiceNumber = GenerateInvoiceNumber(order.OrderId);
                await _context.SaveChangesAsync();
              

                // Step 6: Create order line items
                // Each purchased product becomes one row
                var orderItems = cartItems.Select(item => new OrderItem
                {
                    // Use generated OrderId from saved order header
                    OrderId = order.OrderId,
                    PartId = item.PartID,
                    Quantity = item.Quantity,                   
                    UnitPrice = item.Part?.PPrice ?? 0m, // Store price snapshot at purchase time
                    TotalPrice = item.Quantity * (item.Part?.PPrice ?? 0m) // Total per line item
                    }).ToList();
               
                // Save all line items
                _context.tbl_order_items.AddRange(orderItems);
                await _context.SaveChangesAsync();

                // Step 7: Commit transaction
                // Both tables persist successfully
                _context.tbl_cart.RemoveRange(cartItems);//clear cart table
                await _context.SaveChangesAsync();// save then proceed transcation commit
                await transaction.CommitAsync();//Without commit, the API may return success while DB silently rolls back at dispose.

                // Step 8: Return response DTO for frontend
                return new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    InvoiceNumber = order.InvoiceNumber,
                    TotalAmount = order.TotalAmount
                };
            }
            catch(Exception ex)
            {
                // Rollback everything if any step fails
                await transaction.RollbackAsync();
                throw new Exception("Order placement failed. Transaction rolled back.",ex);
            }
           }

        private string GenerateInvoiceNumber(int orderId)
        {
            return $"INV-{DateTime.Now:yyyyMMdd}-{orderId:D5}";
        }
    }
}
