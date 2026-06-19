using System.ComponentModel.DataAnnotations;

namespace roster_api_app.DTOs;

public class CommonAreaRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Floor is required.")]
    public int FloorId { get; set; }
}
