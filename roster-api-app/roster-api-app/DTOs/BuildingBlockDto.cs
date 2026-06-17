namespace roster_api_app.DTOs;

public class BuildingBlockDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LocationTypeId { get; set; }
    public string LocationTypeName { get; set; } = string.Empty;
    public List<FloorDto> Floors { get; set; } = [];
}
