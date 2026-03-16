using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.DTOs;
using Microsoft.EntityFrameworkCore;
using carkaashiv_angular_API.Interfaces;



namespace carkaashiv_angular_API.Services
{

    public class PartService : IPartService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly S3UploadServices _s3UploadServices;
    public PartService(AppDbContext context, IHttpContextAccessor accessor, S3UploadServices s3UploadServices)
    {
        _context = context;
        _httpContextAccessor = accessor;
        _s3UploadServices = s3UploadServices;

    }
    public async Task<PartResponseDto> CreatePartAsync(PartCreateDto dto)
    {
            var userIdClaim = _httpContextAccessor.HttpContext?
               .User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");
            var part = new Part
        {
            PEmpId = int.Parse(userIdClaim),
            PName = dto.Name,
            PDetail = dto.Description,
            PPrice = dto.Price,
            PStock = dto.Stock,
            ImagePath = await _s3UploadServices.FinalizeImageAsync(dto.ImageKey,null)
            // CreatedAt = DateTime.UtcNow  // db fill this column
        };
        _context.tbl_part.Add(part);
        await _context.SaveChangesAsync();
        return MapToDto(part);
    }

        public async Task<PartResponseDto?> UpdatePartAsync(int id,PartUpdateDto dto)
        {
            var part = await _context.tbl_part.FindAsync(id);
            if (part == null) return null;

            var userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");
            
            part.PEmpId = int.Parse(userIdClaim);
            part.PName = dto.Name;
            part.PDetail = dto.Description;
            part.PPrice = dto.Price;
            part.PStock = dto.stock;
            if (!string.IsNullOrEmpty(dto.ImageKey))//no S3 call Update Without Image Change
            {
                part.ImagePath = await _s3UploadServices.FinalizeImageAsync(dto.ImageKey, part.ImagePath);
            }            
            part.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapToDto(part);
        }

        public async Task<List<PartResponseDto>> GetAllPartsAsync()
        {
            return await _context.tbl_part
                .Select(p => MapToDto(p))
                .ToListAsync();
        }
        public async Task<PartResponseDto?> GetPartByIdAsync(int id)
        {
            var part = await _context.tbl_part
                .FirstOrDefaultAsync(p => p.PartId == id);

            if (part == null) return null;
            return MapToDto(part);        
        }
        public async Task<bool> DeletePartAsync(int id)
        {
            var part = await _context.tbl_part.FindAsync(id);
            if (part == null) return false;
            _context.tbl_part.Remove(part);
            await _context.SaveChangesAsync();
            return true;
        }

        private static PartResponseDto MapToDto(Part p)
        {
            return new PartResponseDto
            {
                Id = p.PartId,
                Name = p.PName ?? "",
                Description = p.PDetail ?? "",
                Price = p.PPrice ?? 0,
                stock = p.PStock ?? 0,
                ImageUrl = p.ImagePath ?? ""
            };
        }  
     }

}