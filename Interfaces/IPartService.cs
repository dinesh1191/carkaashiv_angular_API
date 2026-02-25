using carkaashiv_angular_API.DTOs;

namespace carkaashiv_angular_API.Interfaces
{
    public interface IPartService
    {
        Task<PartResponseDto> CreatePartAsync(PartCreateDto dto);
        Task<PartResponseDto?>UpdatePartAsync(int id,  PartUpdateDto dto);
        Task<List<PartResponseDto>> GetAllPartsAsync();        
        Task<PartResponseDto?> GetPartByIdAsync(int id);
        Task<bool> DeletePartAsync(int d);
    }
}
