namespace roster_api_app.Entities;

public class CommonArea
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int FloorId { get; set; }
    public Floor Floor { get; set; } = null!;
}
