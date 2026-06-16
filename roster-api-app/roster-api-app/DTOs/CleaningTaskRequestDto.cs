namespace roster_api_app.DTOs;

using System.ComponentModel.DataAnnotations;
using roster_api_app.Entities.Enums;

public class CleaningTaskRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Task category is required.")]
    [EnumDataType(typeof(CleaningTaskCategory))]
    public CleaningTaskCategory? TaskCategory { get; set; }

    [Range(1, 1440)]
    public int EstimatedDuration { get; set; }

    [Required]
    [StringLength(50)]
    public string Frequency { get; set; } = string.Empty;
}
