namespace roster_api_app.DTOs;

public class ResidentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public string CleaningFrequency { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}