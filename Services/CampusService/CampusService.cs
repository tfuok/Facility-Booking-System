using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.ModelExtensions;
using Repositories.Models;
using Repositories.Repo;
using Services.UserService;

namespace Services.CampusService
{
    public class CampusService
    {
        private readonly CampusRepository _repo;
        private readonly ILogger<CampusService> _logger;
        private readonly ICurrentUserService _currentUser;

        public CampusService(CampusRepository repo, ILogger<CampusService> logger, ICurrentUserService currentUser)
        {
            _repo = repo;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<Campus?> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<List<Campus>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<int> CreateAsync(Campus entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (await _repo.ExistsByNameAsync(entity.Name))
                throw new InvalidOperationException("Tên Campus đã tồn tại.");

            try
            {
                var userId = _currentUser.UserId;

                entity.CreatedBy = userId;
                entity.CreatedAt = DateTime.UtcNow;
                return await _repo.CreateAsync(entity);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới Campus");
                throw new InvalidOperationException("Không thể tạo bản ghi mới.", ex);
            }
        }

        public async Task<int> UpdateAsync(Campus entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (await _repo.ExistsByNameAsync(entity.Name, entity.Id))
                throw new InvalidOperationException("Tên Campus đã tồn tại.");

            try
            {
                var userId = _currentUser.UserId;

                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
                return await _repo.UpdateAsync(entity);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật Campus");
                throw new InvalidOperationException("Không thể cập nhật bản ghi.", ex);
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _repo.ExistsAsync(id);
        }


        public async Task<PaginationResult<List<CampusDto>>> GetPagingAsync(
            int currentPage, int pageSize, string? name, string? address)
        {
            var result = await _repo.GetPagingAsync(currentPage, pageSize, name, address);

            var dtoItems = result.Items.Select(campus => new CampusDto
            {
                Id = campus.Id,
                Name = campus.Name,
                Address = campus.Address,
                Description = campus.Description
            }).ToList();

            return new PaginationResult<List<CampusDto>>
            {
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                Items = dtoItems
            };
        }

        public async Task<int> SoftDeleteAsync(string id)
        {
            var userId = _currentUser.UserId;
            return await _repo.SoftDeleteAsync(id, userId);
        }
    }
}
