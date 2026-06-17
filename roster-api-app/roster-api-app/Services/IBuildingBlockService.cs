using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface IBuildingBlockService
{
    Task<IEnumerable<BuildingBlockDto>> GetAllAsync();
    Task<BuildingBlockDto?> GetByIdAsync(int id);
    Task<BuildingBlockDto> CreateAsync(BuildingBlockRequestDto dto);
    Task UpdateAsync(int id, BuildingBlockRequestDto dto);
    Task DeleteAsync(int id);
}
