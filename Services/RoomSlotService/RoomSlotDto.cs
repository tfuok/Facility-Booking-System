using Repositories.Models.Enums;

namespace Services.RoomSlotService
{
    public class RoomSlotDto
    {
        public string Id { get; set; } = default!;
        public string RoomId { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string SlotType { get; set; } = default!;
        public string Status { get; set; } = default!;
    }

    public class RoomSlotCreateRequest
    {
        public string RoomId { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public RoomSlotType SlotType { get; set; }
        public RoomSlotStatus Status { get; set; }
    }

    public class RoomSlotUpdateRequest
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public RoomSlotType SlotType { get; set; }
        public RoomSlotStatus Status { get; set; }
    }
}
