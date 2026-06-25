using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using Azure;
using Azure.Core;
using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Exceptions;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Middleware;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Enums;
using carkaashiv_angular_API.Models.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace carkaashiv_angular_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly S3UploadServices _s3UploadServices;


        public OrderService(AppDbContext context, S3UploadServices s3UploadServices)
        {
            _context = context;
            _s3UploadServices = s3UploadServices;
        }
        public async Task<OrderResponseDto> PlaceOrderAsync(int currentUserId,PlaceOrderRequest request, string idempotencyKey)
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

                //Step 2.Validate Cart(empty / removed items and delivery address)
                if (!cartItems.Any())
                    throw new BusinessException("Your cart is empty. Some items may have been removed or are no longer available.");

                if (string.IsNullOrWhiteSpace(request.DeliveryName))
                {
                    throw new BusinessException("Delivery name is required");
                }

                if (string.IsNullOrWhiteSpace(request.DeliveryPhone))
                {
                    throw new BusinessException("Delivery phone is required");
                }

                if (string.IsNullOrWhiteSpace(request.DeliveryAddress))
                {
                    throw new BusinessException("Delivery address is required");
                }
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
                    User = null!, // Navigation property initialized via UserId; suppress nullable warning
                    SubtotalAmount = subtotal,
                    TaxAmount = tax,
                    TotalAmount = total,
                    DeliveryName = request.DeliveryName,
                    DeliveryPhone = request.DeliveryPhone,
                    DeliveryAddress = request.DeliveryAddress,
                    Landmark = request.Landmark,
                    OrderStatus = OrderStatus.Pending,
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
                //Step 3: Unknown → bubble up handle by middleware
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
            if (order.UserId != currentUserId)
                throw new UnauthorizedAccessException("Access denied");

            return new OrderDetailDto
            {
                OrderId = order.OrderId,
                InvoiceNumber = order.InvoiceNumber ?? string.Empty,
                CreatedAt = order.CreatedAt,
                Status = order.OrderStatus.ToString(),
                SubtotalAmount = order.SubtotalAmount,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    PartName = i.Part?.PName ?? string.Empty,
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
                TotalAmount = order.TotalAmount,
                deliveryName = order.DeliveryName,
                deliveryPhone = order.DeliveryPhone,
                deliveryAddress = order.DeliveryAddress,
                landmark = order.Landmark??"",
            };
        }
        public static class DbExceptionHelper
        {
            public static bool IsUniqueConstraintViolation(DbUpdateException ex)
            {
                return ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505";
            }
        }

        public async Task<SubmitPaymentResult> SubmitPaymentAsync(int userId, int orderId, SubmitPaymentRequest request)
        {
            // validations
            if (string.IsNullOrWhiteSpace(request.TempKey))
                throw new BusinessException("Payment screenshot required");

            var order = await _context.tbl_orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                throw new BusinessException("Order not found");

            if (order.PaymentStatus == PaymentStatus.Submitted)
                throw new BusinessException("Payment already submitted");

            var (proofUrl, proofKey) = await _s3UploadServices.FinalizeImageAsync(request.TempKey, $"payments/order-{orderId}", null);

            // update order
            order.PaymentMethod = request.PaymentMethod;
            order.PaymentReference = request.PaymentReference;
            order.PaymentProofUrl = proofUrl;
            order.PaymentProofKey = proofKey;
            order.PaymentStatus = PaymentStatus.Submitted;
            order.PaymentSubmittedAt = DateTime.UtcNow;

            // insert payment
            var payment = new OrderPayment
            {
                OrderId = orderId,
                Amount = order.TotalAmount,  //System truth for bill amount
                PaymentMethod = request.PaymentMethod,
                PaymentReference = request.PaymentReference

            };
            _context.OrderPayments.Add(payment);
            await _context.SaveChangesAsync(); // single save (atomic)

            // mark order as submitted (not verified yet)
            return new SubmitPaymentResult
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                SubmittedAt = payment.SubmittedAt
            };

        }
        public async Task<VerifyPaymentResult> VerifyPaymentAsync(int orderId)
        {
            var order = await _context.tbl_orders
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order == null)
                throw new BusinessException("Order not found");

            if (order.PaymentStatus != PaymentStatus.Submitted)
                throw new BusinessException("Only submitted payments can be verified");


            //System truth
            var totalPaid = await _context.OrderPayments
                     .Where(x => x.OrderId == orderId)
                      .SumAsync(x => x.Amount);

            var diff = totalPaid - order.TotalAmount;
            order.VerifiedAmount = totalPaid;
            order.PaymentMismatchAmount = diff;
            string verificationLabel;

            // auto decision (still admin-triggered)
            if (diff < 0)
            {
                order.PaymentStatus = PaymentStatus.FailedVerification;
                verificationLabel = "UNDERPAID";
            }
            else if (diff == 0)
            {
                order.PaymentStatus = PaymentStatus.Verified;
                order.OrderStatus = OrderStatus.ReadyForDispatch;
                order.PaymentVerifiedAt = DateTime.UtcNow;
                verificationLabel = "EXACT";
            }
            else
            {
                // Overpaid case
                order.PaymentStatus = PaymentStatus.Verified;
                order.OrderStatus = OrderStatus.ReadyForDispatch;
                order.PaymentVerifiedAt = DateTime.UtcNow;
                verificationLabel = "OVERPAID";
            }
            await _context.SaveChangesAsync();

            return new VerifyPaymentResult
            {
                Label = verificationLabel,
                ExpectedAmount = order.TotalAmount,
                PaidAmount = totalPaid,
                MismatchAmount = diff
            };

        }
        public async Task<List<AdminOrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {

            var query = _context.tbl_orders
            .AsNoTracking() // Read-only query optimization - disables EF Core change tracking
            .Include(x => x.User)
            .Where(x => x.OrderStatus == status);

            if (status == OrderStatus.Pending)
            {
                query = query.Where(x =>
                    x.PaymentStatus == PaymentStatus.Submitted);
            }
            return await query
            .OrderByDescending(x => x.PaymentSubmittedAt)
            .Select(x => new AdminOrderDto
            {
                OrderId = x.OrderId,
                CustomerName = x.User.Name ?? string.Empty,
                CustomerPhone = x.User.Phone?? string.Empty,
                TotalAmount = x.TotalAmount,
                PaymentProofUrl = x.PaymentProofUrl,
                PaymentReference = x.PaymentReference,
                SubmittedAt = x.PaymentSubmittedAt
            }).ToListAsync();
        }

        public async Task MarkAsShippedAsync(int orderId) {

            var order = await _context.tbl_orders.FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order == null)
                throw new BusinessException("Order not found");

            if (order.OrderStatus != OrderStatus.ReadyForDispatch)
                throw new BusinessException("Only dispatch-ready order can be marked as shipped");

            order.OrderStatus = OrderStatus.Shipped;
            await _context.SaveChangesAsync();
        }
    

        //Customer order history
    public async Task<List<MyOrderDto>> GetMyOrdersAsync(int currentUserId)
        {
            return await _context.tbl_orders
                    .AsNoTracking()
                    .Where(x => x.UserId == currentUserId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new MyOrderDto
                    {
                        OrderId = x.OrderId,
                        TotalAmount = x.TotalAmount,
                        RecipientName = x.DeliveryName,
                        RecipientAddress = x.DeliveryAddress,
                        RecipientPhone = x.DeliveryPhone,
                        LandMark = x.Landmark,
                        PaymentProofUrl = x.PaymentProofUrl,
                        CreatedAt = x.CreatedAt,
                        // Can edit if order is still pending OR payment is not finalized
                        CanEditAddress =  x.OrderStatus == OrderStatus.Pending ||
                                          x.PaymentStatus == PaymentStatus.Pending ||
                                          x.PaymentStatus == PaymentStatus.Submitted,
                        OrderStatus = x.OrderStatus,
                        OrderStatusText = x.OrderStatus.ToString()
                    }).ToListAsync();

        }
        public async Task<AdminOrderDetailsDto?> GetOrderDetailsAsync(int OrderId)
        {
            return await _context.tbl_orders
                .Where(o => o.OrderId == OrderId)
                .Select(o => new AdminOrderDetailsDto
                {
                    OrderId = o.OrderId,
                    RecipientName = o.DeliveryName,
                    RecipientPhone = o.DeliveryPhone,
                    RecipientAddress = o.DeliveryAddress,
                    LandMark = o.Landmark?? "",
                    GstTaxAmount = o.TaxAmount,
                    TotalAmount  = o.TotalAmount,
                    SubmittedAt  = o.PaymentSubmittedAt,

                    Items = o.OrderItems
                    .Select(i => new AdminOrderItemDto
                    {
                        PartId = i.PartId,
                        PartName = i.Part.PName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        LineTotal = i.Quantity * i.UnitPrice
                    }).ToList()

                }).FirstOrDefaultAsync();
        }
    }
    
}
