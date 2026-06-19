using Microsoft.EntityFrameworkCore;
using roster_api_app.Data;
using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public class ResidentRepository : IResidentRepository
{
    private readonly ApplicationDbContext _context;

    public ResidentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Resident>> GetAllAsync()
    {
        return await IncludeResidents()
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Resident?> GetByIdAsync(int id)
    {
        return await IncludeResidents().FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(Resident resident)
    {
        _context.Residents.Add(resident);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Resident resident)
    {
        _context.Residents.Update(resident);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var resident = await _context.Residents.FindAsync(id);
        if (resident != null)
        {
            _context.Residents.Remove(resident);
            await _context.SaveChangesAsync();
        }
    }

    private IQueryable<Resident> IncludeResidents()
    {
        return _context.Residents
            .Include(r => r.Unit)
            .ThenInclude(u => u!.Floor)
            .ThenInclude(f => f.BuildingBlock)
            .Include(r => r.Apartment)
            .ThenInclude(a => a!.Floor)
            .ThenInclude(f => f.BuildingBlock);
    }
}
