namespace FacilityBookingSystem.Controllers
{
    public class ApiResponse
    {
        public int errorCode { get; set; }
        public string message { get; set; }
        public object? data { get; set; }
    }
}
