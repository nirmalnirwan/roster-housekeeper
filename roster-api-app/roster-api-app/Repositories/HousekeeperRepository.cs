using Microsoft.EntityFrameworkCore;
using roster_api_app.Data;
using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public class HousekeeperRepository : IHousekeeperRepository
{
    private readonly ApplicationDbContext _context;

    public HousekeeperRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Housekeeper>> GetAllAsync()
    {
        return await _context.Housekeepers.ToListAsync();
    }

    public async Task<Housekeeper?> GetByIdAsync(int id)
    {
        return await _context.Housekeepers.FindAsync(id);
    }

    public async Task AddAsync(Housekeeper housekeeper)
    {
        _context.Housekeepers.Add(housekeeper);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Housekeeper housekeeper)
    {
        _context.Housekeepers.Update(housekeeper);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var housekeeper = await _context.Housekeepers.FindAsync(id);
        if (housekeeper != null)
        {
            _context.Housekeepers.Remove(housekeeper);
            await _context.SaveChangesAsync();
        }
    }
}