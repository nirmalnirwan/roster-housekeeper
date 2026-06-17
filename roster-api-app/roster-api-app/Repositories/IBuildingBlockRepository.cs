using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface IBuildingBlockRepository
{
    Task<IEnumerable<BuildingBlock>> GetAllAsync();
    Task<BuildingBlock?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(int locationTypeId, string name, int? excludeId = null);
    Task AddAsync(BuildingBlock buildingBlock);
    Task UpdateAsync(BuildingBlock buildingBlock);
    Task DeleteAsync(int id);
}
