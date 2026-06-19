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
        return await IncludeRosterGraph(_context.Rosters)
            .OrderByDescending(r => r.WeekStartDate)
            .ToListAsync();
    }

    public async Task<Roster?> GetByIdAsync(int id)
    {
        return await IncludeRosterGraph(_context.Rosters)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Roster?> GetByWeekStartDateAsync(DateTime weekStartDate)
    {
        var normalized = weekStartDate.Date;
        return await IncludeRosterGraph(_context.Rosters)
            .FirstOrDefaultAsync(r => r.WeekStartDate.Date == normalized);
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

    private static IQueryable<Roster> IncludeRosterGraph(IQueryable<Roster> query)
    {
        return query
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Housekeeper)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Task)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Location)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.CommonArea)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Unit)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Apartment)
            .Include(r => r.RosterTasks)
            .ThenInclude(rt => rt.Resident);
    }
}
