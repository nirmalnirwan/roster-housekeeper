using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface IUnitService
{
    Task<IEnumerable<UnitDto>> GetAllAsync();
    Task<IEnumerable<UnitDto>> GetByFloorAsync(int floorId);
    Task<UnitDto?> GetByIdAsync(int id);
    Task<UnitDto> CreateAsync(UnitRequestDto dto);
    Task UpdateAsync(int id, UnitRequestDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<AssignableAreaDto>> GetAssignableAreasAsync();
}
