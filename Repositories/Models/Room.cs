namespace Repositories.Models
{
    public class Room : BaseEntity
    {
        public required string RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }
        public required string AreaId { get; set; }
        public Area? Area { get; set; }
        public required string RoomNumber { get; set; }
        public string? RoomName { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public ICollection<RoomSlot>? RoomSlots { get; set; }
    }
}
