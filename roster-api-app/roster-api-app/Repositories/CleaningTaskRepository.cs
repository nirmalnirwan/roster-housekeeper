using Microsoft.EntityFrameworkCore;
using roster_api_app.Data;
using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public class CleaningTaskRepository : ICleaningTaskRepository
{
    private readonly ApplicationDbContext _context;

    public CleaningTaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CleaningTask>> GetAllAsync()
    {
        return await _context.CleaningTasks
            .OrderBy(t => t.TaskCategory)
            .ThenBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<CleaningTask?> GetByIdAsync(int id)
    {
        return await _context.CleaningTasks.FindAsync(id);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();

        return await _context.CleaningTasks.AnyAsync(t =>
            t.Name.ToLower() == normalizedName &&
            (!excludeId.HasValue || t.Id != excludeId.Value));
    }

    public async Task AddAsync(CleaningTask task)
    {
        _context.CleaningTasks.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CleaningTask task)
    {
        _context.CleaningTasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _context.CleaningTasks.FindAsync(id);
        if (task != null)
        {
            _context.CleaningTasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
