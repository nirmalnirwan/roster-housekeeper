namespace roster_api_app.Entities;

public class Floor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public int BuildingBlockId { get; set; }
    public BuildingBlock BuildingBlock { get; set; } = null!;
}
