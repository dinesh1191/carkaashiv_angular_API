using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.DTOs;
using Microsoft.EntityFrameworkCore;


namespace carkaashiv_angular_API.Services
{
    
    public class PartService
    {
        private readonly AppDbContext _context;
        public PartService(AppDbContext context)
        { 
            _context = context;
          
        }
        public async Task<TablePart> CreatePartAsync(PartCreateDto dto)
        {
            var part = new TablePart
            {
                PEmpId = dto.EmployeeId,
                PName = dto.Name,
                PDetail = dto.Description,
                PPrice = dto.Price,
                PStock = dto.Stock,
                ImagePath = "Placeholder",
                CreatedAt = DateTime.UtcNow
            };
            _context.tbl_part.Add(part);
            await _context.SaveChangesAsync();
             
            return part;
        }

        public async Task<TablePart?> UpdatePartAsync(int id, PartUpdateDto dto)
        {
            var part = await _context.tbl_part.FindAsync(id);
            if(part == null)
            {
                return null;
            }
            part.PName = dto.Name;
            part.PDetail = dto.Description;
            part.PPrice = dto.Price;
            part.PStock = dto.stock;
            await _context.SaveChangesAsync();
            return part;
        }
        
        public async Task<List<PartResponseDto>> GetAllPartsAsync()
        {
            return await _context.tbl_part
                .Select(p => new PartResponseDto
                {
                    Id = p.PartId,
                    Name = p.PName!,
                    Description = p.PDetail!,
                    Price = p.PPrice ?? 0,
                    stock = p.PStock ?? 0,
                    ImageUrl = p.ImagePath!


                })
                .ToListAsync();
        }

        public async Task<PartResponseDto?> GetPartByIdAsync(int id)
        {
            return await _context.tbl_part
                .Where(p=> p.PartId ==id)
                .Select(p => new PartResponseDto
                {
                    Id = p.PartId,
                    Name = p.PName!,
                    Description = p.PDetail!,
                    Price = p.PPrice ?? 0,
                    stock = p.PStock ?? 0,
                    ImageUrl = p.ImagePath!
                }).FirstOrDefaultAsync();
        }

        public async Task<bool>DeletePartAsync(int id)
        {
            var part = await _context.tbl_part.FindAsync(id);
            if (part == null) return false;
            _context.tbl_part.Remove(part);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
