using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.RoomTypeService;

namespace FacilityBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly RoomTypeService _roomTypeService;

        public RoomTypeController(RoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }

        public sealed record RoomTypeRequest(string name, string description);

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RoomTypeRequest input)
        {
            var roomType = new RoomType
            {
                Name = input.name,
                Description = input.description
            };

            try
            {
                await _roomTypeService.CreateAsync(roomType);
                return Ok(new ApiResponse
                {
                    errorCode = 0,
                    message = "Room Type created successfully!",
                    data = roomType
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    errorCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] RoomTypeRequest input)
        {
            try
            {
                var existing = await _roomTypeService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ApiResponse
                    {
                        errorCode = 404,
                        message = "RoomType not found"
                    });
                }

                existing.Name = input.name;
                existing.Description = input.description;

                await _roomTypeService.UpdateAsync(existing);

                return Ok(new ApiResponse
                {
                    errorCode = 0,
                    message = "RoomType updated successfully!",
                    data = existing
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    errorCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _roomTypeService.GetByIdAsync(id);

            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    errorCode = 404,
                    message = "RoomType not found"
                });
            }

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = item
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null)
        {
            var result = await _roomTypeService.GetPagingAsync(currentPage, pageSize, name);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = result
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _roomTypeService.SoftDeleteAsync(id);

                return Ok(new ApiResponse
                {
                    errorCode = 0,
                    message = "Deleted successfully",
                    data = id
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
