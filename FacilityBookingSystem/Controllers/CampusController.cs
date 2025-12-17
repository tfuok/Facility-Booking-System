using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.CampusService;

namespace FacilityBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly CampusService _campusService;

        public CampusController(CampusService campusService)
        {
            _campusService = campusService;
        }

        public sealed record CampusCreateRequest(string name, string address, string description);
        public sealed record CampusUpdateRequest(string name, string address, string description);

        // ======================= CREATE =======================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CampusCreateRequest input)
        {
            var campus = new Campus
            {
                Name = input.name,
                Address = input.address,
                Description = input.description
            };

            try
            {
                await _campusService.CreateAsync(campus);

                return Ok(new ApiResponse
                {
                    errorCode = 0,
                    message = "Campus created successfully!",
                    data = campus
                });
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(new ApiResponse
                {
                    errorCode = 400,
                    message = ex.Message,
                    data = null
                });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new ApiResponse
                {
                    errorCode = 500,
                    message = "Internal server error",
                    data = ex.Message
                });
            }
        }

        // ======================= UPDATE =======================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] CampusUpdateRequest input)
        {
            try
            {
                var existing = await _campusService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ApiResponse
                    {
                        errorCode = 404,
                        message = "Campus not found",
                        data = null
                    });
                }

                existing.Name = input.name;
                existing.Address = input.address;
                existing.Description = input.description;

                await _campusService.UpdateAsync(existing);

                return Ok(new ApiResponse
                {
                    errorCode = 0,
                    message = "Campus updated successfully!",
                    data = existing
                });
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(new ApiResponse
                {
                    errorCode = 400,
                    message = ex.Message,
                    data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    errorCode = 500,
                    message = "Internal server error",
                    data = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var campus = await _campusService.GetByIdAsync(id);

            if (campus == null)
            {
                return NotFound(new ApiResponse
                {
                    errorCode = 404,
                    message = "Campus not found"
                });
            }

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = campus
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? address = null)
        {
            var result = await _campusService.GetPagingAsync(currentPage, pageSize, name, address);

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
                await _campusService.SoftDeleteAsync(id);

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
