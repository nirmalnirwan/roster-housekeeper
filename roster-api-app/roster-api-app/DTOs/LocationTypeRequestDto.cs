namespace roster_api_app.DTOs;

using System.ComponentModel.DataAnnotations;

public class LocationTypeRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}
