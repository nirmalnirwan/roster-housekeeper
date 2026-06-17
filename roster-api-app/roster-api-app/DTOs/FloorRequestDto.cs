namespace roster_api_app.DTOs;

using System.ComponentModel.DataAnnotations;

public class FloorRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 200)]
    public int FloorNumber { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Building block is required.")]
    public int BuildingBlockId { get; set; }
}
