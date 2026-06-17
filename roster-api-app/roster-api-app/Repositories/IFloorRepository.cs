using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface IFloorRepository
{
    Task<IEnumerable<Floor>> GetAllAsync();
    Task<Floor?> GetByIdAsync(int id);
    Task<bool> FloorNumberExistsAsync(int buildingBlockId, int floorNumber, int? excludeId = null);
    Task AddAsync(Floor floor);
    Task UpdateAsync(Floor floor);
    Task DeleteAsync(int id);
}
