namespace roster_api_app.Entities;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty; // Room, Gym, Cinema, etc.
    public string Building { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}