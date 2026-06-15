namespace roster_api_app.DTOs;

using System.ComponentModel.DataAnnotations;
using roster_api_app.Entities.Enums;

public class HousekeeperDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be Active or Inactive.")]
    public string Status { get; set; } = string.Empty;

    [EnumDataType(typeof(EmployeeTypes))]
    public EmployeeTypes EmploymentType { get; set; } = EmployeeTypes.Permanent;
}
