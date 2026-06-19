namespace roster_api_app.Entities;

public class Apartment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int FloorId { get; set; }
    public Floor Floor { get; set; } = null!;
    public ICollection<Resident> Residents { get; set; } = new List<Resident>();
}
