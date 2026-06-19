using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface IResidentService
{
    Task<IEnumerable<ResidentDto>> GetAllAsync();
    Task<ResidentDto?> GetByIdAsync(int id);
    Task<ResidentDto> CreateAsync(ResidentRequestDto dto);
    Task UpdateAsync(int id, ResidentRequestDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<AssignableAreaDto>> GetAssignableAreasAsync();
}
