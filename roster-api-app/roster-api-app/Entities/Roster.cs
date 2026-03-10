using System;
using System.Collections.Generic;

namespace roster_api_app.Entities;

public class Roster
{
    public int Id { get; set; }
    public DateTime WeekStartDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public ICollection<RosterTask> RosterTasks { get; set; } = new List<RosterTask>();
}