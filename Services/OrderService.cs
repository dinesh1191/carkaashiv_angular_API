using Amazon.S3.Model;
using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Diagnostics;
using System.Reflection.Metadata;
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
            // Check → Process → Persist → Handle race → Return

           
            // Step 0: Fetch all cart items for the current user and begin transactions
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 0: Fetch cart
                var cartItems = await _context.tbl_cart
                .Where(c => c.UId == userId)
                .Include(c => c.Part)
                .ToListAsync();

                if (!cartItems.Any())
                    throw new ArgumentException("Cart is empty");

                // Step:1  Fast path — check existing order
                var existingOrderId = await _context.OrderIdempotencies
                    .Where(x => x.UserId == userId && x.IdempotencyKey == idempotencyKey)
                    .Select(x => x.OrderId)
                    .FirstOrDefaultAsync();

                if (existingOrderId > 0)
                {
                    var existingOrder = await GetOrderOrThrow(existingOrderId);
                    return MapToResponse(existingOrder);
                }

                // Step 2: Calculate totals
                var subtotal = cartItems.Sum(x => x.Quantity * (x.Part?.PPrice ?? 0m));
                var tax = subtotal * 0.18m;  // Apply 18% tax              
                var total = subtotal + tax;  // Final payable amount

                // Step 3:Create order          
                var order = new Order
                {
                    UserId = userId,
                    SubtotalAmount = subtotal,
                    TaxAmount = tax,
                    TotalAmount = total,
                    Status = "Completed",
                    InvoiceNumber = string.Empty
                };
                _context.tbl_orders.Add(order);
                await _context.SaveChangesAsync();

                // Step 4: Generate invoice
                order.InvoiceNumber = GenerateInvoiceNumber(order.OrderId);
                await _context.SaveChangesAsync();

                // Step 5: Lock idempotency early (important)
                _context.OrderIdempotencies.Add(new OrderIdempotency
                {
                    UserId = userId,
                    IdempotencyKey = idempotencyKey,
                    OrderId = order.OrderId
                });
                await _context.SaveChangesAsync();


                // Step 6: Save order items

                var orderItems = cartItems.Select(item => new OrderItem
                {
                    OrderId = order.OrderId,//Use generated OrderId from saved order header
                    PartId = item.PartID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Part?.PPrice ?? 0m, // Store price snapshot at purchase time
                    TotalPrice = item.Quantity * (item.Part?.PPrice ?? 0m) // Total per line item
                });

                _context.tbl_order_items.AddRange(orderItems);// Save all line items
                
                // Step 7:Clear cart
                _context.tbl_cart.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                // Step 8: Commit            
                await transaction.CommitAsync(); // Without commit, the API may return success while DB silently rolls back at dispose.
                return MapToResponse(order); //Return response DTO for frontend
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();// Rollback everything if any step fails


                var existingKey = await _context.OrderIdempotencies
                    .FirstOrDefaultAsync(x =>
                    x.UserId == userId && x.IdempotencyKey == idempotencyKey);

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
        public static class DbExceptionHelper
        {
            public static bool IsUniqueConstraintViolation(DbUpdateException ex)
            {
                return ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505";
            }
        }

    } 
}
