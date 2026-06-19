using Microsoft.EntityFrameworkCore;
using roster_api_app.Data;
using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public class AreaRepository : IAreaRepository
{
    private readonly ApplicationDbContext _context;

    public AreaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CommonArea>> GetCommonAreasAsync()
    {
        return await IncludeCommonAreas().ToListAsync();
    }

    public async Task<IEnumerable<CommonArea>> GetCommonAreasByFloorAsync(int floorId)
    {
        return await IncludeCommonAreas().Where(ca => ca.FloorId == floorId).ToListAsync();
    }

    public async Task<CommonArea?> GetCommonAreaByIdAsync(int id)
    {
        return await IncludeCommonAreas().FirstOrDefaultAsync(ca => ca.Id == id);
    }

    public async Task<bool> CommonAreaNameExistsAsync(int floorId, string name, int? excludeId = null)
    {
        return await _context.CommonAreas.AnyAsync(ca =>
            ca.FloorId == floorId &&
            ca.Name.ToLower() == name.ToLower() &&
            (!excludeId.HasValue || ca.Id != excludeId.Value));
    }

    public async Task AddCommonAreaAsync(CommonArea commonArea)
    {
        _context.CommonAreas.Add(commonArea);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCommonAreaAsync(CommonArea commonArea)
    {
        _context.CommonAreas.Update(commonArea);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommonAreaAsync(int id)
    {
        var commonArea = await _context.CommonAreas.FindAsync(id);
        if (commonArea != null)
        {
            _context.CommonAreas.Remove(commonArea);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Unit>> GetUnitsAsync()
    {
        return await IncludeUnits().ToListAsync();
    }

    public async Task<IEnumerable<Unit>> GetUnitsByFloorAsync(int floorId)
    {
        return await IncludeUnits().Where(u => u.FloorId == floorId).ToListAsync();
    }

    public async Task<Unit?> GetUnitByIdAsync(int id)
    {
        return await IncludeUnits().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> UnitNumberExistsAsync(int floorId, string unitNumber, int? excludeId = null)
    {
        return await _context.Units.AnyAsync(u =>
            u.FloorId == floorId &&
            u.UnitNumber.ToLower() == unitNumber.ToLower() &&
            (!excludeId.HasValue || u.Id != excludeId.Value));
    }

    public async Task AddUnitAsync(Unit unit)
    {
        _context.Units.Add(unit);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUnitAsync(Unit unit)
    {
        _context.Units.Update(unit);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUnitAsync(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        if (unit != null)
        {
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Apartment>> GetApartmentsAsync()
    {
        return await IncludeApartments().ToListAsync();
    }

    public async Task<IEnumerable<Apartment>> GetApartmentsByFloorAsync(int floorId)
    {
        return await IncludeApartments().Where(a => a.FloorId == floorId).ToListAsync();
    }

    public async Task<Apartment?> GetApartmentByIdAsync(int id)
    {
        return await IncludeApartments().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> ApartmentNumberExistsAsync(int floorId, string apartmentNumber, int? excludeId = null)
    {
        return await _context.Apartments.AnyAsync(a =>
            a.FloorId == floorId &&
            a.ApartmentNumber.ToLower() == apartmentNumber.ToLower() &&
            (!excludeId.HasValue || a.Id != excludeId.Value));
    }

    public async Task AddApartmentAsync(Apartment apartment)
    {
        _context.Apartments.Add(apartment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateApartmentAsync(Apartment apartment)
    {
        _context.Apartments.Update(apartment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteApartmentAsync(int id)
    {
        var apartment = await _context.Apartments.FindAsync(id);
        if (apartment != null)
        {
            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();
        }
    }

    private IQueryable<CommonArea> IncludeCommonAreas()
    {
        return _context.CommonAreas
            .Include(ca => ca.Floor)
            .ThenInclude(f => f.BuildingBlock)
            .ThenInclude(bb => bb.LocationType)
            .OrderBy(ca => ca.Floor.BuildingBlock.LocationTypeId)
            .ThenBy(ca => ca.Floor.BuildingBlock.Name)
            .ThenBy(ca => ca.Floor.FloorNumber)
            .ThenBy(ca => ca.Name);
    }

    private IQueryable<Unit> IncludeUnits()
    {
        return _context.Units
            .Include(u => u.Floor)
            .ThenInclude(f => f.BuildingBlock)
            .ThenInclude(bb => bb.LocationType)
            .OrderBy(u => u.Floor.BuildingBlock.LocationTypeId)
            .ThenBy(u => u.Floor.BuildingBlock.Name)
            .ThenBy(u => u.Floor.FloorNumber)
            .ThenBy(u => u.UnitNumber);
    }

    private IQueryable<Apartment> IncludeApartments()
    {
        return _context.Apartments
            .Include(a => a.Floor)
            .ThenInclude(f => f.BuildingBlock)
            .ThenInclude(bb => bb.LocationType)
            .OrderBy(a => a.Floor.BuildingBlock.LocationTypeId)
            .ThenBy(a => a.Floor.BuildingBlock.Name)
            .ThenBy(a => a.Floor.FloorNumber)
            .ThenBy(a => a.ApartmentNumber);
    }
}
