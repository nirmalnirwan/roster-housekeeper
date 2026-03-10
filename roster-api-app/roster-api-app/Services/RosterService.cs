using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;
using System.Linq;

namespace roster_api_app.Services;

public class RosterService : IRosterService
{
    private readonly IRosterRepository _repository;

    public RosterService(IRosterRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RosterDto>> GetAllAsync()
    {
        var rosters = await _repository.GetAllAsync();
        return rosters.Select(MapToDto);
    }

    public async Task<RosterDto?> GetByIdAsync(int id)
    {
        var roster = await _repository.GetByIdAsync(id);
        if (roster == null) return null;
        return MapToDto(roster);
    }

    public async Task<RosterDto> CreateAsync(RosterDto dto)
    {
        var roster = new Roster
        {
            WeekStartDate = dto.WeekStartDate,
            CreatedBy = dto.CreatedBy,
            CreatedDate = dto.CreatedDate,
            RosterTasks = dto.RosterTasks.Select(t => new RosterTask
            {
                HousekeeperId = t.HousekeeperId,
                TaskId = t.TaskId,
                LocationId = t.LocationId,
                ResidentId = t.ResidentId,
                ScheduledDate = t.ScheduledDate,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                FrequencyType = t.FrequencyType,
                Notes = t.Notes
            }).ToList()
        };
        await _repository.AddAsync(roster);
        dto.Id = roster.Id;
        return dto;
    }

    public async Task UpdateAsync(int id, RosterDto dto)
    {
        var roster = await _repository.GetByIdAsync(id);
        if (roster == null) throw new KeyNotFoundException("Roster not found");
        roster.WeekStartDate = dto.WeekStartDate;
        roster.CreatedBy = dto.CreatedBy;
        roster.CreatedDate = dto.CreatedDate;
        // for simplicity replace tasks entirely
        roster.RosterTasks.Clear();
        roster.RosterTasks = dto.RosterTasks.Select(t => new RosterTask
        {
            HousekeeperId = t.HousekeeperId,
            TaskId = t.TaskId,
            LocationId = t.LocationId,
            ResidentId = t.ResidentId,
            ScheduledDate = t.ScheduledDate,
            StartTime = t.StartTime,
            EndTime = t.EndTime,
            FrequencyType = t.FrequencyType,
            Notes = t.Notes
        }).ToList();
        await _repository.UpdateAsync(roster);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<byte[]> ExportPdfAsync(int rosterId)
    {
        // stub implementation - real PDF generation would be done here
        return System.Text.Encoding.UTF8.GetBytes($"Roster {rosterId} PDF export");
    }

    public async Task<byte[]> ExportExcelAsync(int rosterId)
    {
        // stub implementation - real Excel generation would be done here
        return System.Text.Encoding.UTF8.GetBytes($"Roster {rosterId} Excel export");
    }

    private RosterDto MapToDto(Roster r)
    {
        return new RosterDto
        {
            Id = r.Id,
            WeekStartDate = r.WeekStartDate,
            CreatedBy = r.CreatedBy,
            CreatedDate = r.CreatedDate,
            RosterTasks = r.RosterTasks.Select(t => new RosterTaskDto
            {
                Id = t.Id,
                RosterId = t.RosterId,
                HousekeeperId = t.HousekeeperId,
                HousekeeperName = t.Housekeeper?.Name ?? string.Empty,
                TaskId = t.TaskId,
                TaskName = t.Task?.Name ?? string.Empty,
                LocationId = t.LocationId,
                LocationName = t.Location?.Name,
                ResidentId = t.ResidentId,
                ResidentName = t.Resident?.Name,
                ScheduledDate = t.ScheduledDate,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                FrequencyType = t.FrequencyType,
                Notes = t.Notes
            }).ToList()
        };
    }
}