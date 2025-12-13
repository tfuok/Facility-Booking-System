using Repositories.Models.Enums;

namespace Services.BookingService
{
    public class BookingDto
    {
        public string Id { get; set; } = default!;

        // Room (summary)
        public string RoomNumber { get; set; } = default!;
        public string? RoomName { get; set; }
        public string AreaName { get; set; } = default!;
        public string RoomTypeName { get; set; } = default!;

        // Slot (summary)
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SlotType { get; set; } = default!;
        public string SlotStatus { get; set; } = default!;

        public string BookingStatus { get; set; } = default!;

        // Booking info
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = default!;
    }

    public class BookingCreateRequest
    {
        public string RoomSlotId { get; set; } = default!;
    }

    public class BookingUpdateRequest
    {
        public string RoomSlotId { get; set; } = default!;
        public string? Note { get; set; }
    }

    public class BookingStatusUpdateRequest
    {
        public BookingStatus Status { get; set; }
    }

}
