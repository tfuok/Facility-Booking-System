using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class RoomType : BaseEntity
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Room>? Rooms { get; set; }
    }
}
