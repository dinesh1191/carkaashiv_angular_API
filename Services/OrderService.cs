using Amazon.S3.Model;
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
        public async Task<OrderResponseDto> PlaceOrderAsync(int userId, string idempotencyKey)
        {
            // checkout lifecycle: Fetch idempotency → if exists return else ;Fetch cart → calculate totals → save bill header → save bill items → clear cart -> commit -> return response
            //Step:0 Idempotency OrderId check
            var existingOrderId = await _context.OrderIdempotencies
                .Where(x => x.UserId == userId && x.IdempotencyKey == idempotencyKey)
                .Select(x=> x.OrderId)
                .FirstOrDefaultAsync();

            if (existingOrderId != 0)
            {
                var order = await GetOrderOrThrow(existingOrderId);
                return MapToResponse(order);
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
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


                // Step 6: Create order items + store idempotency
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

                _context.OrderIdempotencies.Add(new OrderIdempotency
                {
                    UserId = userId,
                    IdempotencyKey = idempotencyKey,
                    OrderId = order.OrderId
                });         
              
                // Step 7: Clear cart table and commit transaction              
                _context.tbl_cart.RemoveRange(cartItems);               
                await _context.SaveChangesAsync(); // save first then proceed transcation commit
                await transaction.CommitAsync(); // Without commit, the API may return success while DB silently rolls back at dispose.

                // Step 8: Return response DTO for frontend

                return MapToResponse(order);               
            }
            catch (DbUpdateException)
            {
                // Rollback everything if any step fails
                await transaction.RollbackAsync();

                var existingKey = await _context.OrderIdempotencies
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.IdempotencyKey == idempotencyKey);

                if (existingKey != null)
                {
                    var order = await GetOrderOrThrow(existingKey.OrderId);
                    return MapToResponse(order);
                }
                throw;
            }
        }

        private string GenerateInvoiceNumber(int orderId)
        {
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{orderId:D5}";
        }


      

        private async Task<Order> GetOrderOrThrow(int orderId)
        {
            var order = await _context.tbl_orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new Exception($"Order {orderId} not found");

            return order;
        }
        private static OrderResponseDto MapToResponse(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                InvoiceNumber = order.InvoiceNumber ?? "",
                TotalAmount = order.TotalAmount
            };
        }


    } 


}
