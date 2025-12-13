using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.BookingService;

namespace FacilityBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _service;

        public BookingController(BookingService service)
        {
            _service = service;
        }

        // ---------------------- GET BY ID ----------------------
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var booking = await _service.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new ApiResponse
                {
                    errorCode = 404,
                    message = "Booking not found"
                });
            }

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = booking
            });
        }

        // ---------------------- PAGING ----------------------
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Paging(
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null)
        {
            var result = await _service.GetPagingAsync(currentPage, pageSize, keyword);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = result
            });
        }

        // ---------------------- CREATE ----------------------
        [HttpPost]
        [Authorize(Roles = "Student,Lecturer")]
        public async Task<IActionResult> Create([FromBody] BookingCreateRequest request)
        {
            var affected = await _service.CreateAsync(request);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Booking created successfully",
                data = new { affected }
            });
        }

        // ---------------------- UPDATE ----------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Lecturer")]
        public async Task<IActionResult> Update(
            string id,
            [FromBody] BookingUpdateRequest request)
        {
            var affected = await _service.UpdateAsync(id, request);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Booking updated successfully",
                data = new { affected }
            });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(
    string id,
    [FromBody] BookingStatusUpdateRequest request)
        {
            var affected = await _service.UpdateStatusAsync(id, request);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Booking status updated successfully",
                data = new { affected }
            });
        }


        // ---------------------- DELETE (SOFT) ----------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student,Lecturer,Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var affected = await _service.DeleteAsync(id);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Booking deleted successfully",
                data = new { affected }
            });
        }
    }
}