namespace roster_api_app.DTOs;

public class AssignableAreaDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string AreaType { get; set; } = string.Empty;
    public int FloorId { get; set; }
    public string FloorName { get; set; } = string.Empty;
    public int BuildingBlockId { get; set; }
    public string BuildingBlockName { get; set; } = string.Empty;
    public int LocationTypeId { get; set; }
    public string LocationTypeName { get; set; } = string.Empty;
}
