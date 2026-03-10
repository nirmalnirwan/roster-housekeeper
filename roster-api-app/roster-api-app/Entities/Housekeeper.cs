namespace roster_api_app.Entities;

public class Housekeeper
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Active, Inactive
    public string EmploymentType { get; set; } = string.Empty; // Full-time, Part-time
}