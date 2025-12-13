namespace Services.AreaService
{
    public class AreaDto
    {
        public string Id { get; set; } = default!;
        public string CampusId { get; set; } = default!;
        public string ManagerId { get; set; } = default!;
        public string Name { get; set; } = default!;
    }

    public class AreaCreateDto
    {
        public string CampusId { get; set; } = default!;
        public string ManagerId { get; set; } = default!;
        public string Name { get; set; } = default!;
    }

    public class AreaUpdateDto
    {
        public string CampusId { get; set; } = default!;
        public string ManagerId { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
