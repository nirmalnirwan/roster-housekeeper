using System;
using System.Collections.Generic;

namespace roster_api_app.DTOs;

public class RosterDto
{
    public int Id { get; set; }
    public DateTime WeekStartDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public List<RosterTaskDto> RosterTasks { get; set; } = new List<RosterTaskDto>();
}