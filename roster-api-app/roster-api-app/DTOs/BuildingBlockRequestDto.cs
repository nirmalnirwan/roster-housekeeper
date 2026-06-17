namespace roster_api_app.DTOs;

using System.ComponentModel.DataAnnotations;

public class BuildingBlockRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Location type is required.")]
    public int LocationTypeId { get; set; }
}
