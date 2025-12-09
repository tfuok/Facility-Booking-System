using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class Area : BaseEntity
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string CampusId { get; set; }
        public Campus? Campus { get; set; }
        public required string ManagerId { get; set; }
        public User? Manager { get; set; }
        public ICollection<Room>? Rooms { get; set; }
    }
}
