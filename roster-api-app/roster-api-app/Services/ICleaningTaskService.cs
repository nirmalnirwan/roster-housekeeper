using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface ICleaningTaskService
{
    Task<IEnumerable<CleaningTaskDto>> GetAllAsync();
    Task<CleaningTaskDto?> GetByIdAsync(int id);
    Task<CleaningTaskDto> CreateAsync(CleaningTaskRequestDto dto);
    Task UpdateAsync(int id, CleaningTaskRequestDto dto);
    Task DeleteAsync(int id);
}
