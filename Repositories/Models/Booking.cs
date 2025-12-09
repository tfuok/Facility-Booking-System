using Repositories.Models.Enums;

namespace Repositories.Models
{
    public class Booking : BaseEntity
    {
        public BookingStatus Status { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public string? RoomId { get; set; }
        public Room? Room { get; set; }
        public string? RoomSlotId { get; set; }
        public RoomSlot? RoomSlot { get; set; }
        public string? RejectReason { get; set; }

    }
}
