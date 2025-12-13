using Microsoft.Extensions.Logging;
using Repositories.ModelExtensions;
using Repositories.Models;
using Repositories.Repo;
using Services.UserService;

namespace Services.RoomService
{
    public class RoomService
    {
        private readonly RoomRepository _repo;
        private readonly ILogger<RoomService> _logger;
        private readonly ICurrentUserService _currentUser;

        public RoomService(RoomRepository repo, ILogger<RoomService> logger, ICurrentUserService currentUser)
        {
            _repo = repo;
            _logger = logger;
            _currentUser = currentUser;
        }


        // ---------------------- GET BY ID ----------------------
        public async Task<RoomDto?> GetByIdAsync(string id)
        {
            var room = await _repo.GetByIdAsync(id);

            if (room == null || room.DeletedAt != null)
                return null;

            return ToDto(room);
        }

        // ---------------------- PAGING ----------------------
        public async Task<PaginationResult<List<RoomDto>>> GetPagingAsync(
            int page, int size, string? keyword)
        {
            var result = await _repo.GetPagingAsync(page, size, keyword);

            var dtoItems = result.Items.Select(x => ToDto(x)).ToList();

            return new PaginationResult<List<RoomDto>>
            {
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                Items = dtoItems
            };
        }

        // ---------------------- CREATE ----------------------
        public async Task<int> CreateAsync(RoomRequest request)
        {
            var room = new Room
            {
                Id = Guid.NewGuid().ToString(),
                RoomTypeId = request.RoomTypeId,
                AreaId = request.AreaId,
                RoomNumber = request.RoomNumber,
                RoomName = request.RoomName,
                Floor = request.Floor,
                Capacity = request.Capacity,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId
            };

            return await _repo.CreateAsync(room);
        }

        // ---------------------- UPDATE ----------------------
        public async Task<int> UpdateAsync(string id, RoomRequest request)
        {
            var existing = await _repo.GetByIdAsync(id);

            if (existing == null || existing.DeletedAt != null)
                throw new KeyNotFoundException("Room không tồn tại hoặc đã bị xóa.");

            existing.RoomTypeId = request.RoomTypeId;
            existing.AreaId = request.AreaId;
            existing.RoomNumber = request.RoomNumber;
            existing.RoomName = request.RoomName;
            existing.Floor = request.Floor;
            existing.Capacity = request.Capacity;
            existing.Description = request.Description;

            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = _currentUser.UserId;

            return await _repo.UpdateAsync(existing);
        }

        // ---------------------- SOFT DELETE ----------------------
        public async Task<int> DeleteAsync(string id)
        {
            return await _repo.SoftDeleteAsync(id, _currentUser.UserId);
        }

        // ---------------------- HELPER MAPPING ----------------------
        private RoomDto ToDto(Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType?.Name,
                AreaId = room.AreaId,
                AreaName = room.Area?.Name,
                RoomNumber = room.RoomNumber,
                RoomName = room.RoomName,
                Floor = room.Floor
                //Capacity = room.Capacity,
                //Description = room.Description
            };
        }
    }
}
