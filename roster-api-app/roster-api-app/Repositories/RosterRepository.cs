using Microsoft.EntityFrameworkCore;
using roster_api_app.Data;
using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public class RosterRepository : IRosterRepository
{
    private readonly ApplicationDbContext _context;

    public RosterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Roster>> GetAllAsync()
    {
        return await _context.Rosters
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Housekeeper)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Task)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Location)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Resident)
            .ToListAsync();
    }

    public async Task<Roster?> GetByIdAsync(int id)
    {
        return await _context.Rosters
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Housekeeper)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Task)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Location)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Resident)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(Roster roster)
    {
        _context.Rosters.Add(roster);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Roster roster)
    {
        _context.Rosters.Update(roster);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var roster = await _context.Rosters.FindAsync(id);
        if (roster != null)
        {
            _context.Rosters.Remove(roster);
            await _context.SaveChangesAsync();
        }
    }
}