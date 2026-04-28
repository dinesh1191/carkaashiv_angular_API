using Amazon.S3.Model;
using Azure;
using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Exceptions;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Middleware;
using carkaashiv_angular_API.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.ComponentModel;
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
        public async Task<OrderResponseDto> PlaceOrderAsync(int currentUserId, string idempotencyKey)
        {            
            // Order Flow:
            // Fetch → Validate → Idempotency → Calculate → Create Order →
            // Deduct Stock → Save Items → Clear Cart → Commit → Return

            // Step 0: Begin transactions
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1: Fetch cart
                var cartItems = await _context.tbl_cart
                .Where(c => c.UId == currentUserId)
                .Include(c => c.Part)
                .ToListAsync();

               //Step 2.Validate Cart(empty / removed items)
                if (!cartItems.Any())
                    throw new BusinessException("Your cart is empty. Some items may have been removed or are no longer available.");

                // Step:3 Check Idempotency(prevent duplicate order)
                var existingOrderId = await _context.OrderIdempotencies
                    .Where(x => x.UserId == currentUserId && x.IdempotencyKey == idempotencyKey)
                    .Select(x => x.OrderId)
                    .FirstOrDefaultAsync();

                if (existingOrderId > 0)
                {
                    var existingOrder = await GetOrderOrThrow(existingOrderId);
                    return MapToResponse(existingOrder);
                }

                // Step 4: Calculate totals(subtotal + tax)
                var subtotal = cartItems.Sum(x => x.Quantity * (x.Part?.PPrice ?? 0m));
                var tax = subtotal * 0.18m;  // Apply 18% tax              
                var total = subtotal + tax;  // Final payable amount

                // Step 5:Create Order(Save → get OrderId)          
                var order = new Order
                {
                    UserId = currentUserId,
                    SubtotalAmount = subtotal,
                    TaxAmount = tax,
                    TotalAmount = total,
                    Status = "Completed",
                    InvoiceNumber = string.Empty
                };
                _context.tbl_orders.Add(order);
                // Save order first (need ID)
                await _context.SaveChangesAsync();
                // Step 5.1: Generate invoice with orderId
                order.InvoiceNumber = GenerateInvoiceNumber(order.OrderId); 
                
                // Step 6: Validate & Deduct Stock(NO save inside loop)               
                foreach (var item in cartItems)
                {
                    var part = item.Part;
                    if (part == null)
                        throw new Exception($"Part not found:{item.PartID}");
                    var availableStock = part.PStock;
                    if (availableStock < item.Quantity)
                        throw new Exception($"Insufficient stock for:{part.PName}");
                    // Deduct stock
                    part.PStock -= item.Quantity;
                }
                 // Step 7: Save order items
                var orderItems = cartItems.Select(item => new OrderItem
                {
                    OrderId = order.OrderId,//Use generated OrderId from saved order header
                    PartId = item.PartID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Part?.PPrice ?? 0m, // Store price snapshot at purchase time
                    TotalPrice = item.Quantity * (item.Part?.PPrice ?? 0m) // Total per line item
                });
                _context.tbl_order_items.AddRange(orderItems);// Save all line items
                // Step 8: Insert Idempotency record
                _context.OrderIdempotencies.Add(new OrderIdempotency
                {
                    UserId = currentUserId,
                    IdempotencyKey = idempotencyKey,
                    OrderId = order.OrderId
                });

                // Step 9:Clear Cart
                _context.tbl_cart.RemoveRange(cartItems);
                //Step 10:SaveChanges(single batch)                
                await _context.SaveChangesAsync();
                // Step 11: .Commit Transaction          
                await transaction.CommitAsync(); // Without commit, the API may return success while DB silently rolls back at dispose.
                return MapToResponse(order); //Return response DTO for frontend

            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();// Rollback everything if any step fails
                
                // Step 1: Idempotency recovery (keep this)
                var existingKey = await _context.OrderIdempotencies
                    .FirstOrDefaultAsync(x =>
                    x.UserId == currentUserId && x.IdempotencyKey == idempotencyKey);

                if (existingKey != null)
                {
                    var order = await GetOrderOrThrow(existingKey.OrderId);
                    return MapToResponse(order);
                }
                // Step 2: Handle known DB issues gracefully
                if (ex.InnerException?.Message.Contains("FK_tbl_cart_tbl_part_part_id") == true)

                {
                    throw new BusinessException("Items were removed because they are no longer available");
                }
                //Step 3: Unknown → bubble up handle byy middleware
                throw;
            }
        }

        public async Task<OrderDetailDto> GetOrderByIdAsync(int currentUserId, int orderId)
        {  
            var order = await _context.tbl_orders
                        .Include(o => o.OrderItems)
                        .ThenInclude(i => i.Part)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found");
            // Authorization check after retrieval
            if (order?.UserId != currentUserId)
                throw new UnauthorizedAccessException("Access denied");

            return new OrderDetailDto
            {
                OrderId = order.OrderId,
                InvoiceNumber = order.InvoiceNumber ?? string.Empty,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                SubtotalAmount = order.SubtotalAmount,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    PartName = i.Part.PName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

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
