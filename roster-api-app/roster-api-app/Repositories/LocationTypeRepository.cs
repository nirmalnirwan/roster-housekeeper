using Microsoft.EntityFrameworkCore;
using roster_api_app.Data;
using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public class LocationTypeRepository : ILocationTypeRepository
{
    private readonly ApplicationDbContext _context;

    public LocationTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LocationType>> GetAllAsync()
    {
        return await _context.LocationTypes
            .Include(lt => lt.BuildingBlocks.OrderBy(bb => bb.Name))
            .ThenInclude(bb => bb.Floors.OrderBy(f => f.FloorNumber).ThenBy(f => f.Name))
            .OrderBy(lt => lt.Id)
            .ToListAsync();
    }

    public async Task<LocationType?> GetByIdAsync(int id)
    {
        return await _context.LocationTypes
            .Include(lt => lt.BuildingBlocks.OrderBy(bb => bb.Name))
            .ThenInclude(bb => bb.Floors.OrderBy(f => f.FloorNumber).ThenBy(f => f.Name))
            .FirstOrDefaultAsync(lt => lt.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();

        return await _context.LocationTypes.AnyAsync(lt =>
            lt.Name.ToLower() == normalizedName &&
            (!excludeId.HasValue || lt.Id != excludeId.Value));
    }

    public async Task AddAsync(LocationType locationType)
    {
        _context.LocationTypes.Add(locationType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LocationType locationType)
    {
        _context.LocationTypes.Update(locationType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var locationType = await _context.LocationTypes.FindAsync(id);
        if (locationType != null)
        {
            _context.LocationTypes.Remove(locationType);
            await _context.SaveChangesAsync();
        }
    }
}
