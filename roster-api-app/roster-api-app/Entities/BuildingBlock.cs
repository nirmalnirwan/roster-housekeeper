namespace roster_api_app.Entities;

public class BuildingBlock
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LocationTypeId { get; set; }
    public LocationType LocationType { get; set; } = null!;
    public ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
