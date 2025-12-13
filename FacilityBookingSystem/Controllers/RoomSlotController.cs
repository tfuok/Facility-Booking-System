using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.RoomSlotService;

namespace FacilityBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomSlotController : ControllerBase
    {
        private readonly RoomSlotService _service;

        public RoomSlotController(RoomSlotService service)
        {
            _service = service;
        }

        // ---------------------- GET BY ID ----------------------
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetById(string id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
            {
                return new ApiResponse
                {
                    errorCode = 1,
                    message = "RoomSlot không tồn tại",
                    data = null
                };
            }

            return new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = data
            };
        }

        // ---------------------- PAGING + SEARCH ----------------------
        [HttpGet]
        public async Task<ApiResponse> GetPaging(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? keyword = null)
        {
            var result = await _service.GetPagingAsync(page, size, keyword);

            return new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = result
            };
        }

        // ---------------------- CREATE ----------------------
        [HttpPost]
        public async Task<ApiResponse> Create([FromBody] RoomSlotCreateRequest request)
        {
            var affected = await _service.CreateAsync(request);

            return new ApiResponse
            {
                errorCode = 0,
                message = "Create success",
                data = new { success = affected > 0 }
            };
        }

        // ---------------------- UPDATE ----------------------
        [HttpPut("{id}")]
        public async Task<ApiResponse> Update(
            string id,
            [FromBody] RoomSlotUpdateRequest request)
        {
            try
            {
                var affected = await _service.UpdateAsync(id, request);

                return new ApiResponse
                {
                    errorCode = 0,
                    message = "Update success",
                    data = new { success = affected > 0 }
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new ApiResponse
                {
                    errorCode = 1,
                    message = ex.Message,
                    data = null
                };
            }
        }

        // ---------------------- DELETE (SOFT) ----------------------
        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(string id)
        {
            try
            {
                var affected = await _service.DeleteAsync(id);

                return new ApiResponse
                {
                    errorCode = 0,
                    message = "Delete success",
                    data = new { success = affected > 0 }
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new ApiResponse
                {
                    errorCode = 1,
                    message = ex.Message,
                    data = null
                };
            }
        }
    }
}