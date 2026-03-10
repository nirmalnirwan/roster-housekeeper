namespace roster_api_app.Entities;

public class CleaningTask
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; } // in minutes
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Fortnightly, Monthly
}