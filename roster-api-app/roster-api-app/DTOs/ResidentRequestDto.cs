using System.ComponentModel.DataAnnotations;

namespace roster_api_app.DTOs;

public class ResidentRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string CleaningFrequency { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
    public int? UnitId { get; set; }
    public int? ApartmentId { get; set; }
}
