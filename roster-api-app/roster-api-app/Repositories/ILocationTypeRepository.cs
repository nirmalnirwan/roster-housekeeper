using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface ILocationTypeRepository
{
    Task<IEnumerable<LocationType>> GetAllAsync();
    Task<LocationType?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task AddAsync(LocationType locationType);
    Task UpdateAsync(LocationType locationType);
    Task DeleteAsync(int id);
}
