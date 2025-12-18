using Repositories.Models.Enums;

namespace Services.UserService
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public Role? Role { get; set; }
    }
}
