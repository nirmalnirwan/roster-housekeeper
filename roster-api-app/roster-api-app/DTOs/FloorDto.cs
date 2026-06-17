namespace roster_api_app.DTOs;

public class FloorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public int BuildingBlockId { get; set; }
    public string BuildingBlockName { get; set; } = string.Empty;
}
