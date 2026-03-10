
using roster_auth_app.Utilities.Enums;

namespace roster_auth_app.Dtos.Users
{
    public class ChangeUserStatusDto
    {
        public string Email { get; set; } = default!;
        public UserStatus Status { get; set; }
    }

}
