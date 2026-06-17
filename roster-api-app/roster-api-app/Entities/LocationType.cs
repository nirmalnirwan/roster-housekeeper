namespace roster_api_app.Entities;

public class LocationType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<BuildingBlock> BuildingBlocks { get; set; } = new List<BuildingBlock>();
}
