using Repositories.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class User : BaseEntity
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public string? Password { get; set; }
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public Role? Role { get; set; }

        public ICollection<Area>? ManagedAreas { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
