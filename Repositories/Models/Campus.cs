using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class Campus : BaseEntity
    {
        [Required]
        public required string Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
    }
}
