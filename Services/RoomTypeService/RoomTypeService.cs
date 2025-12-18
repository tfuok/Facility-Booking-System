using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.ModelExtensions;
using Repositories.Models;
using Repositories.Repo;
using Services.UserService;

namespace Services.RoomTypeService
{
    public class RoomTypeService
    {
        private readonly RoomTypeRepository _repo;
        private readonly ILogger<RoomTypeService> _logger;
        private readonly ICurrentUserService _currentUser;

        public RoomTypeService(RoomTypeRepository repo, ILogger<RoomTypeService> logger, ICurrentUserService currentUser)
        {
            _repo = repo;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<RoomType?> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<List<RoomType>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<int> CreateAsync(RoomType entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (await _repo.ExistsByNameAsync(entity.Name))
                throw new InvalidOperationException("Tên RoomType đã tồn tại.");

            try
            {
                var userId = _currentUser.UserId;

                entity.CreatedBy = userId;
                entity.CreatedAt = DateTime.UtcNow;
                return await _repo.CreateAsync(entity);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới RoomType");
                throw new InvalidOperationException("Không thể tạo bản ghi mới.", ex);
            }
        }

        public async Task<int> UpdateAsync(RoomType entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (await _repo.ExistsByNameAsync(entity.Name, entity.Id))
                throw new InvalidOperationException("Tên RoomType đã tồn tại.");

            try
            {
                var userId = _currentUser.UserId;

                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
                return await _repo.UpdateAsync(entity);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật RoomType");
                throw new InvalidOperationException("Không thể cập nhật bản ghi.", ex);
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _repo.ExistsAsync(id);
        }

        public async Task<PaginationResult<List<RoomType>>> GetPagingAsync(
            int currentPage, int pageSize, string? name)
        {
            return await _repo.GetPagingAsync(currentPage, pageSize, name);
        }

        public async Task<int> SoftDeleteAsync(string id)
        {
            var userId = _currentUser.UserId;
            return await _repo.SoftDeleteAsync(id, userId);
        }
    }
}
