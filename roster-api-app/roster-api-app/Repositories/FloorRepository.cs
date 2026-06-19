using Microsoft.EntityFrameworkCore;
using roster_api_app.Data;
using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public class FloorRepository : IFloorRepository
{
    private readonly ApplicationDbContext _context;

    public FloorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Floor>> GetAllAsync()
    {
        return await _context.Floors
            .Include(f => f.BuildingBlock)
            .OrderBy(f => f.BuildingBlockId)
            .ThenBy(f => f.FloorNumber)
            .ThenBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<Floor?> GetByIdAsync(int id)
    {
        return await _context.Floors
            .Include(f => f.BuildingBlock)
            .ThenInclude(bb => bb.LocationType)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Floor>> GetByBuildingBlockAsync(int buildingBlockId)
    {
        return await _context.Floors
            .Include(f => f.BuildingBlock)
            .Where(f => f.BuildingBlockId == buildingBlockId)
            .OrderBy(f => f.FloorNumber)
            .ThenBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<bool> FloorNumberExistsAsync(int buildingBlockId, int floorNumber, int? excludeId = null)
    {
        return await _context.Floors.AnyAsync(f =>
            f.BuildingBlockId == buildingBlockId &&
            f.FloorNumber == floorNumber &&
            (!excludeId.HasValue || f.Id != excludeId.Value));
    }

    public async Task AddAsync(Floor floor)
    {
        _context.Floors.Add(floor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Floor floor)
    {
        _context.Floors.Update(floor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var floor = await _context.Floors.FindAsync(id);
        if (floor != null)
        {
            _context.Floors.Remove(floor);
            await _context.SaveChangesAsync();
        }
    }
}
