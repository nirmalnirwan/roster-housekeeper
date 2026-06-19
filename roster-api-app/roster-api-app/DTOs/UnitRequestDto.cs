using System.ComponentModel.DataAnnotations;

namespace roster_api_app.DTOs;

public class UnitRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UnitNumber { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Floor is required.")]
    public int FloorId { get; set; }
}
