using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class RosterService : IRosterService
{
    private readonly IRosterRepository _repository;
    private readonly IHousekeeperRepository _housekeeperRepository;
    private readonly ICleaningTaskRepository _cleaningTaskRepository;
    private readonly IAreaRepository _areaRepository;

    public RosterService(
        IRosterRepository repository,
        IHousekeeperRepository housekeeperRepository,
        ICleaningTaskRepository cleaningTaskRepository,
        IAreaRepository areaRepository)
    {
        _repository = repository;
        _housekeeperRepository = housekeeperRepository;
        _cleaningTaskRepository = cleaningTaskRepository;
        _areaRepository = areaRepository;
    }

    public async Task<IEnumerable<RosterDto>> GetAllAsync()
    {
        var rosters = await _repository.GetAllAsync();
        return rosters.Select(MapToDto);
    }

    public async Task<RosterDto?> GetByIdAsync(int id)
    {
        var roster = await _repository.GetByIdAsync(id);
        return roster == null ? null : MapToDto(roster);
    }

    public async Task<RosterDto?> GetByWeekStartDateAsync(DateTime weekStartDate)
    {
        var roster = await _repository.GetByWeekStartDateAsync(weekStartDate);
        return roster == null ? null : MapToDto(roster);
    }

    public async Task<RosterDto> CreateAsync(RosterDto dto)
    {
        Normalize(dto);
        await ValidateRosterAsync(dto);

        var existing = await _repository.GetByWeekStartDateAsync(dto.WeekStartDate);
        if (existing != null)
            throw new InvalidOperationException("A roster already exists for this week.");

        var roster = new Roster
        {
            WeekStartDate = dto.WeekStartDate,
            CreatedBy = dto.CreatedBy,
            CreatedDate = dto.CreatedDate == default ? DateTime.UtcNow : dto.CreatedDate,
            RosterTasks = dto.RosterTasks.Select(MapToEntity).ToList()
        };

        await _repository.AddAsync(roster);
        var created = await _repository.GetByIdAsync(roster.Id);
        return MapToDto(created!);
    }

    public async Task UpdateAsync(int id, RosterDto dto)
    {
        var roster = await _repository.GetByIdAsync(id);
        if (roster == null) throw new KeyNotFoundException("Roster not found");

        Normalize(dto);
        await ValidateRosterAsync(dto);

        var existingForWeek = await _repository.GetByWeekStartDateAsync(dto.WeekStartDate);
        if (existingForWeek != null && existingForWeek.Id != id)
            throw new InvalidOperationException("A roster already exists for this week.");

        roster.WeekStartDate = dto.WeekStartDate;
        roster.CreatedBy = dto.CreatedBy;
        roster.CreatedDate = dto.CreatedDate == default ? roster.CreatedDate : dto.CreatedDate;
        roster.RosterTasks.Clear();
        roster.RosterTasks = dto.RosterTasks.Select(MapToEntity).ToList();

        await _repository.UpdateAsync(roster);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<byte[]> ExportPdfAsync(int rosterId)
    {
        return await Task.FromResult(System.Text.Encoding.UTF8.GetBytes($"Roster {rosterId} PDF export"));
    }

    public async Task<byte[]> ExportExcelAsync(int rosterId)
    {
        return await Task.FromResult(System.Text.Encoding.UTF8.GetBytes($"Roster {rosterId} Excel export"));
    }

    private async Task ValidateRosterAsync(RosterDto dto)
    {
        if (dto.WeekStartDate == default)
            throw new InvalidOperationException("Week start date is required.");

        foreach (var task in dto.RosterTasks)
        {
            await ValidateRosterTaskAsync(task);
        }

        var overlaps = dto.RosterTasks
            .GroupBy(t => new { t.HousekeeperId, Date = t.ScheduledDate.Date })
            .SelectMany(group => group
                .OrderBy(t => t.StartTime)
                .Zip(group.OrderBy(t => t.StartTime).Skip(1), (current, next) => new { current, next }))
            .Any(pair => pair.current.EndTime > pair.next.StartTime);

        if (overlaps)
            throw new InvalidOperationException("A housekeeper cannot have overlapping roster tasks.");
    }

    private async Task ValidateRosterTaskAsync(RosterTaskDto task)
    {
        if (task.HousekeeperId <= 0)
            throw new InvalidOperationException("Housekeeper is required for every roster task.");

        if (await _housekeeperRepository.GetByIdAsync(task.HousekeeperId) == null)
            throw new InvalidOperationException("Roster task references an invalid housekeeper.");

        if (task.TaskId <= 0)
            throw new InvalidOperationException("Cleaning task is required for every roster task.");

        if (await _cleaningTaskRepository.GetByIdAsync(task.TaskId) == null)
            throw new InvalidOperationException("Roster task references an invalid cleaning task.");

        if (task.ScheduledDate == default)
            throw new InvalidOperationException("Scheduled date is required for every roster task.");

        if (task.StartTime >= task.EndTime)
            throw new InvalidOperationException("Roster task start time must be before end time.");

        var assignedAreaCount = new[] { task.CommonAreaId, task.UnitId, task.ApartmentId }.Count(id => id.HasValue);
        if (assignedAreaCount != 1)
            throw new InvalidOperationException("Each roster task must be assigned to one cleaning area.");

        if (task.CommonAreaId.HasValue && await _areaRepository.GetCommonAreaByIdAsync(task.CommonAreaId.Value) == null)
            throw new InvalidOperationException("Roster task references an invalid common area.");

        if (task.UnitId.HasValue && await _areaRepository.GetUnitByIdAsync(task.UnitId.Value) == null)
            throw new InvalidOperationException("Roster task references an invalid unit.");

        if (task.ApartmentId.HasValue && await _areaRepository.GetApartmentByIdAsync(task.ApartmentId.Value) == null)
            throw new InvalidOperationException("Roster task references an invalid apartment.");
    }

    private static void Normalize(RosterDto dto)
    {
        dto.WeekStartDate = dto.WeekStartDate.Date;
        dto.CreatedBy = dto.CreatedBy.Trim();
        dto.RosterTasks = dto.RosterTasks.Select(task =>
        {
            task.ScheduledDate = task.ScheduledDate.Date;
            task.FrequencyType = task.FrequencyType.Trim();
            task.Notes = task.Notes.Trim();
            return task;
        }).ToList();
    }

    private static RosterTask MapToEntity(RosterTaskDto task)
    {
        return new RosterTask
        {
            Id = task.Id,
            HousekeeperId = task.HousekeeperId,
            TaskId = task.TaskId,
            LocationId = task.LocationId,
            CommonAreaId = task.CommonAreaId,
            UnitId = task.UnitId,
            ApartmentId = task.ApartmentId,
            ResidentId = task.ResidentId,
            ScheduledDate = task.ScheduledDate,
            StartTime = task.StartTime,
            EndTime = task.EndTime,
            FrequencyType = task.FrequencyType,
            Notes = task.Notes
        };
    }

    private static RosterDto MapToDto(Roster roster)
    {
        return new RosterDto
        {
            Id = roster.Id,
            WeekStartDate = roster.WeekStartDate,
            CreatedBy = roster.CreatedBy,
            CreatedDate = roster.CreatedDate,
            RosterTasks = roster.RosterTasks
                .OrderBy(t => t.ScheduledDate)
                .ThenBy(t => t.StartTime)
                .Select(MapTaskToDto)
                .ToList()
        };
    }

    private static RosterTaskDto MapTaskToDto(RosterTask task)
    {
        var areaType = task.CommonAreaId.HasValue ? "CommonArea" : task.UnitId.HasValue ? "Unit" : task.ApartmentId.HasValue ? "Apartment" : string.Empty;
        var areaName = task.CommonArea?.Name ?? task.Unit?.Name ?? task.Apartment?.Name ?? string.Empty;

        return new RosterTaskDto
        {
            Id = task.Id,
            RosterId = task.RosterId,
            HousekeeperId = task.HousekeeperId,
            HousekeeperName = task.Housekeeper?.Name ?? string.Empty,
            TaskId = task.TaskId,
            TaskName = task.Task?.Name ?? string.Empty,
            LocationId = task.LocationId,
            LocationName = task.Location?.Name,
            CommonAreaId = task.CommonAreaId,
            CommonAreaName = task.CommonArea?.Name,
            UnitId = task.UnitId,
            UnitName = task.Unit?.Name,
            ApartmentId = task.ApartmentId,
            ApartmentName = task.Apartment?.Name,
            AreaType = areaType,
            AreaName = areaName,
            ResidentId = task.ResidentId,
            ResidentName = task.Resident?.Name,
            ScheduledDate = task.ScheduledDate,
            StartTime = task.StartTime,
            EndTime = task.EndTime,
            FrequencyType = task.FrequencyType,
            Notes = task.Notes
        };
    }
}

