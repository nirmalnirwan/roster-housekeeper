using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface ICleaningTaskRepository
{
    Task<IEnumerable<CleaningTask>> GetAllAsync();
    Task<CleaningTask?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task AddAsync(CleaningTask task);
    Task UpdateAsync(CleaningTask task);
    Task DeleteAsync(int id);
}
