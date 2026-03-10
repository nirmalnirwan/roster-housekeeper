using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface IHousekeeperRepository
{
    Task<IEnumerable<Housekeeper>> GetAllAsync();
    Task<Housekeeper?> GetByIdAsync(int id);
    Task AddAsync(Housekeeper housekeeper);
    Task UpdateAsync(Housekeeper housekeeper);
    Task DeleteAsync(int id);
}