using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.DTOs;


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
   }
}
