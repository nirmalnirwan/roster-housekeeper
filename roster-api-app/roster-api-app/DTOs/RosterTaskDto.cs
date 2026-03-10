using System;

namespace roster_api_app.DTOs;

public class RosterTaskDto
{
    public int Id { get; set; }
    public int RosterId { get; set; }
    public int HousekeeperId { get; set; }
    public string HousekeeperName { get; set; } = string.Empty;
    public int TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public int? LocationId { get; set; }
    public string? LocationName { get; set; }
    public int? ResidentId { get; set; }
    public string? ResidentName { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string FrequencyType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}