using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface ILocationTypeService
{
    Task<IEnumerable<LocationTypeDto>> GetAllAsync();
    Task<LocationTypeDto?> GetByIdAsync(int id);
    Task<LocationTypeDto> CreateAsync(LocationTypeRequestDto dto);
    Task UpdateAsync(int id, LocationTypeRequestDto dto);
    Task DeleteAsync(int id);
}
