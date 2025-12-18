using Microsoft.AspNetCore.Mvc;
using Services.RoomService;

namespace FacilityBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _service;

        public RoomController(RoomService service)
        {
            _service = service;
        }

        // ---------------------- GET PAGING ----------------------
        [HttpGet]
        public async Task<IActionResult> GetPaging(
            int page = 1,
            int size = 10,
            string? keyword = null)
        {
            var result = await _service.GetPagingAsync(page, size, keyword);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = result
            });
        }

        // ---------------------- GET BY ID ----------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var room = await _service.GetByIdAsync(id);

            if (room == null)
            {
                return NotFound(new ApiResponse
                {
                    errorCode = 404,
                    message = "Room not found"
                });
            }

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = room
            });
        }

        // ---------------------- CREATE ----------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomRequest request)
        {
            try
            {
                var id = await _service.CreateAsync(request);

                return Ok(new ApiResponse
                {
                    errorCode = 0,
                    message = "Created successfully",
                    data = new { id }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    errorCode = 400,
                    message = ex.Message
                });
            }
        }

        // ---------------------- UPDATE ----------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RoomRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);

                return Ok(new ApiResponse
                {
                    errorCode = 0,
                    message = "Updated successfully",
                    data = new { id = result }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse
                {
                    errorCode = 404,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    errorCode = 400,
                    message = ex.Message
                });
            }
        }

        // ---------------------- SOFT DELETE ----------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                return Ok(new ApiResponse
                {
                    errorCode = 0,
                    message = "Deleted successfully",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse
                {
                    errorCode = 404,
                    message = ex.Message
                });
            }
        }
    }
}

