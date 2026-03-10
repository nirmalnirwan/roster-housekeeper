using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface IRosterRepository
{
    Task<IEnumerable<Roster>> GetAllAsync();
    Task<Roster?> GetByIdAsync(int id);
    Task AddAsync(Roster roster);
    Task UpdateAsync(Roster roster);
    Task DeleteAsync(int id);
}