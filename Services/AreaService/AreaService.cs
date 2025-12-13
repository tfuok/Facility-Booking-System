using Microsoft.Extensions.Logging;
using Repositories.ModelExtensions;
using Repositories.Models;
using Repositories.Repo;
using Services.UserService;

namespace Services.AreaService
{
    public interface IAreaService
    {
        Task<Area?> GetByIdAsync(string id);
        Task<PaginationResult<List<AreaDto>>> GetPagingAsync(int page, int size, string? name, string? campusId, string? managerId);
        Task<int> CreateAsync(AreaCreateDto dto);
        Task<int> UpdateAsync(string id, AreaUpdateDto dto);
        Task<int> SoftDeleteAsync(string id);
    }
    public class AreaService : IAreaService
    {
        private readonly AreaRepository _repo;
        private readonly ILogger<AreaService> _logger;
        private readonly ICurrentUserService _currentUser;


        public AreaService(AreaRepository repo, ILogger<AreaService> logger, ICurrentUserService currentUser)
        {
            _repo = repo;
            _logger = logger;
            _currentUser = currentUser;
        }

        // -------------------- GET BY ID --------------------
        public async Task<Area?> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        // -------------------- PAGING --------------------
        public async Task<PaginationResult<List<AreaDto>>> GetPagingAsync(
            int page,
            int size,
            string? name,
            string? campusId,
            string? managerId)
        {
            var result = await _repo.GetPagingAsync(page, size, name, campusId, managerId);

            // Map entity → DTO
            var dtoItems = result.Items.Select(area => new AreaDto
            {
                Id = area.Id,
                CampusId = area.CampusId,
                ManagerId = area.ManagerId,
                Name = area.Name
            }).ToList();

            // Trả về bản cập nhật luôn
            return new PaginationResult<List<AreaDto>>
            {
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                Items = dtoItems
            };
        }


        // -------------------- CREATE --------------------
        public async Task<int> CreateAsync(AreaCreateDto dto)
        {
            var area = new Area
            {
                Id = Guid.NewGuid().ToString(),
                CampusId = dto.CampusId,
                ManagerId = dto.ManagerId,
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId
            };

            return await _repo.CreateAsync(area);
        }

        // -------------------- UPDATE --------------------
        public async Task<int> UpdateAsync(string id, AreaUpdateDto dto)
        {
            var area = await _repo.GetByIdAsync(id);
            if (area == null)
                throw new KeyNotFoundException("Area không tồn tại hoặc đã bị xóa.");

            area.CampusId = dto.CampusId;
            area.ManagerId = dto.ManagerId;
            area.Name = dto.Name;
            area.UpdatedAt = DateTime.UtcNow;
            area.UpdatedBy = _currentUser.UserId;

            return await _repo.UpdateAsync(area);
        }


        // -------------------- SOFT DELETE --------------------
        public async Task<int> SoftDeleteAsync(string id)
        {
            return await _repo.SoftDeleteAsync(id, _currentUser.UserId);
        }
    }
}
