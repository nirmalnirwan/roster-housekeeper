using Microsoft.AspNetCore.Identity;
using roster_auth_app.Utilities.Enums;

namespace roster_auth_app.Models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public UserStatus? Status { get; set; }

        // Deactivation tracking
        public DateTime? DeactivatedAt { get; set; }
        public string? DeactivationReason { get; set; }

        // Force password change on first login (for invited users)
        public bool MustChangePassword { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
