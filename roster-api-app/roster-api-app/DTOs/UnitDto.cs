namespace roster_api_app.DTOs;

public class UnitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int FloorId { get; set; }
    public string FloorName { get; set; } = string.Empty;
    public int BuildingBlockId { get; set; }
    public string BuildingBlockName { get; set; } = string.Empty;
    public int LocationTypeId { get; set; }
    public string LocationTypeName { get; set; } = string.Empty;
}
