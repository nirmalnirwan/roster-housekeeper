namespace roster_api_app.DTOs;

public class CleaningTaskDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; }
    public string Frequency { get; set; } = string.Empty;
}