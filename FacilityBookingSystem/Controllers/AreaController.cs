using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.AreaService;

namespace FacilityBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _service;

        public AreaController(IAreaService service)
        {
            _service = service;
        }

        // -------------------- GET BY ID --------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound(new ApiResponse
                {
                    errorCode = 404,
                    message = "Không tìm thấy Area",
                    data = null
                });
            }

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = data
            });
        }

        // -------------------- PAGING --------------------
        [HttpGet]
        public async Task<IActionResult> Paging(
            int page = 1,
            int size = 10,
            string? name = null,
            string? campusId = null,
            string? managerId = null)
        {
            var result = await _service.GetPagingAsync(page, size, name, campusId, managerId);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = result
            });
        }

        // -------------------- CREATE --------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AreaCreateDto dto)
        {
            var affected = await _service.CreateAsync(dto);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Created successfully",
                data = new { affected }
            });
        }

        // -------------------- UPDATE --------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] AreaUpdateDto dto)
        {
            var affected = await _service.UpdateAsync(id, dto);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Updated successfully",
                data = new { affected }
            });
        }

        // -------------------- DELETE (SOFT) --------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var affected = await _service.SoftDeleteAsync(id);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Deleted successfully",
                data = new { affected }
            });
        }
    }
}
