using Repositories.Models.Enums;

namespace Repositories.Models
{
    public class RoomSlot : BaseEntity
    {
        public required string RoomId { get; set; }
        public Room? Room { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public RoomSlotType SlotType { get; set; }
        public RoomStatus RoomStatus { get; set; }
    }
}