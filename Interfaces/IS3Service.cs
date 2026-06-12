using Amazon.S3;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Models;

namespace carkaashiv_angular_API.Interfaces
{
    public interface IS3Service
    {

        Task<Order?> GetOrderByIdAsync(int orderId);    

        Task SaveChangesAsync();

    }
}
