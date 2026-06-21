using System.ComponentModel.DataAnnotations;

namespace roster_api_app.DTOs;

public class ApartmentRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ApartmentNumber { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Floor is required.")]
    public int FloorId { get; set; }

    [Required(ErrorMessage = "Cleaning task is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Cleaning task is required.")]
    public int? CleaningTaskId { get; set; }
}
