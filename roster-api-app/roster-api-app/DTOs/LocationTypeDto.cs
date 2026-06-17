namespace roster_api_app.DTOs;

public class LocationTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<BuildingBlockDto> BuildingBlocks { get; set; } = [];
}
