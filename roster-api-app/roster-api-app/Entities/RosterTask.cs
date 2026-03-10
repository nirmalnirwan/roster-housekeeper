using System;

namespace roster_api_app.Entities;

public class RosterTask
{
    public int Id { get; set; }
    public int RosterId { get; set; }
    public Roster Roster { get; set; } = null!;
    public int HousekeeperId { get; set; }
    public Housekeeper Housekeeper { get; set; } = null!;
    public int TaskId { get; set; }
    public CleaningTask Task { get; set; } = null!;
    public int? LocationId { get; set; }
    public Location? Location { get; set; }
    public int? ResidentId { get; set; }
    public Resident? Resident { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string FrequencyType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}