namespace Services.RoomService
{
    public class RoomDto
    {
        public string Id { get; set; } = default!;
        public string RoomTypeId { get; set; } = default!;
        public string? RoomTypeName { get; set; }

        public string AreaId { get; set; } = default!;
        public string? AreaName { get; set; }
        public string RoomNumber { get; set; } = default!;
        public string? RoomName { get; set; }
        public int Floor { get; set; }
    }

    public class RoomRequest
    {
        public string RoomTypeId { get; set; } = default!;
        public string AreaId { get; set; } = default!;
        public string RoomNumber { get; set; } = default!;
        public string? RoomName { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public string? Description { get; set; }
    }

}
