using Microsoft.EntityFrameworkCore;
using roster_api_app.Data;
using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public class BuildingBlockRepository : IBuildingBlockRepository
{
    private readonly ApplicationDbContext _context;

    public BuildingBlockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BuildingBlock>> GetAllAsync()
    {
        return await _context.BuildingBlocks
            .Include(bb => bb.LocationType)
            .Include(bb => bb.Floors.OrderBy(f => f.FloorNumber).ThenBy(f => f.Name))
            .OrderBy(bb => bb.LocationTypeId)
            .ThenBy(bb => bb.Name)
            .ToListAsync();
    }

    public async Task<BuildingBlock?> GetByIdAsync(int id)
    {
        return await _context.BuildingBlocks
            .Include(bb => bb.LocationType)
            .Include(bb => bb.Floors.OrderBy(f => f.FloorNumber).ThenBy(f => f.Name))
            .FirstOrDefaultAsync(bb => bb.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.BuildingBlocks.AnyAsync(bb => bb.Id == id);
    }

    public async Task<bool> NameExistsAsync(int locationTypeId, string name, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();

        return await _context.BuildingBlocks.AnyAsync(bb =>
            bb.LocationTypeId == locationTypeId &&
            bb.Name.ToLower() == normalizedName &&
            (!excludeId.HasValue || bb.Id != excludeId.Value));
    }

    public async Task AddAsync(BuildingBlock buildingBlock)
    {
        _context.BuildingBlocks.Add(buildingBlock);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BuildingBlock buildingBlock)
    {
        _context.BuildingBlocks.Update(buildingBlock);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var buildingBlock = await _context.BuildingBlocks.FindAsync(id);
        if (buildingBlock != null)
        {
            _context.BuildingBlocks.Remove(buildingBlock);
            await _context.SaveChangesAsync();
        }
    }
}
