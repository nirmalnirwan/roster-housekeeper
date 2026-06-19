using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface IResidentRepository
{
    Task<IEnumerable<Resident>> GetAllAsync();
    Task<Resident?> GetByIdAsync(int id);
    Task AddAsync(Resident resident);
    Task UpdateAsync(Resident resident);
    Task DeleteAsync(int id);
}
