using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface IRosterRepository
{
    Task<IEnumerable<Roster>> GetAllAsync();
    Task<Roster?> GetByIdAsync(int id);
    Task<Roster?> GetByHousekeeperAndWeekAsync(int housekeeperId, DateTime weekStartDate);
    Task AddAsync(Roster roster);
    Task UpdateAsync(Roster roster);
    Task DeleteAsync(int id);
}
