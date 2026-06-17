using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface IFloorService
{
    Task<IEnumerable<FloorDto>> GetAllAsync();
    Task<FloorDto?> GetByIdAsync(int id);
    Task<FloorDto> CreateAsync(FloorRequestDto dto);
    Task UpdateAsync(int id, FloorRequestDto dto);
    Task DeleteAsync(int id);
}
