namespace roster_api_app.Entities;

using roster_api_app.Entities.Enums;

public class CleaningTask
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CleaningTaskCategory TaskCategory { get; set; } = CleaningTaskCategory.CommunityArea;
    public int EstimatedDuration { get; set; } // in minutes
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Fortnightly, Monthly
    public ICollection<CommonArea> CommonAreas { get; set; } = new List<CommonArea>();
    public ICollection<Unit> Units { get; set; } = new List<Unit>();
    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}
