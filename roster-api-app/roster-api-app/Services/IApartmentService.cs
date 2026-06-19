using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface IApartmentService
{
    Task<IEnumerable<ApartmentDto>> GetAllAsync();
    Task<IEnumerable<ApartmentDto>> GetByFloorAsync(int floorId);
    Task<ApartmentDto?> GetByIdAsync(int id);
    Task<ApartmentDto> CreateAsync(ApartmentRequestDto dto);
    Task UpdateAsync(int id, ApartmentRequestDto dto);
    Task DeleteAsync(int id);
}
