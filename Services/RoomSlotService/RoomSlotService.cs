using Microsoft.Extensions.Logging;
using Repositories.ModelExtensions;
using Repositories.Models;
using Repositories.Repo;
using Services.UserService;

namespace Services.RoomSlotService
{
    public class RoomSlotService
    {
        private readonly RoomSlotRepository _repo;
        private readonly ILogger<RoomSlotService> _logger;
        private readonly ICurrentUserService _currentUser;

        public RoomSlotService(
            RoomSlotRepository repo,
            ILogger<RoomSlotService> logger,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _logger = logger;
            _currentUser = currentUser;
        }

        // ---------------------- GET BY ID ----------------------
        public async Task<RoomSlotDto?> GetByIdAsync(string id)
        {
            var slot = await _repo.GetByIdAsync(id);

            if (slot == null || slot.DeletedAt != null)
                return null;

            return ToDto(slot);
        }

        // ---------------------- PAGING ----------------------
        public async Task<PaginationResult<List<RoomSlotDto>>> GetPagingAsync(
            int page, int size, string? keyword)
        {
            var result = await _repo.GetPagingAsync(page, size, keyword);

            var dtoItems = result.Items.Select(ToDto).ToList();

            return new PaginationResult<List<RoomSlotDto>>
            {
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                Items = dtoItems
            };
        }

        // ---------------------- CREATE ----------------------
        public async Task<int> CreateAsync(RoomSlotCreateRequest request)
        {
            var entity = new RoomSlot
            {
                Id = Guid.NewGuid().ToString(),
                RoomId = request.RoomId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                SlotType = request.SlotType,
                RoomStatus = request.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId
            };

            return await _repo.CreateAsync(entity);
        }

        // ---------------------- UPDATE ----------------------
        public async Task<int> UpdateAsync(string id, RoomSlotUpdateRequest request)
        {
            var existing = await _repo.GetByIdAsync(id);

            if (existing == null || existing.DeletedAt != null)
                throw new KeyNotFoundException("RoomSlot không tồn tại hoặc đã bị xóa.");

            existing.StartTime = request.StartTime;
            existing.EndTime = request.EndTime;
            existing.SlotType = request.SlotType;
            existing.RoomStatus = request.Status;

            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = _currentUser.UserId;

            return await _repo.UpdateAsync(existing);
        }

        // ---------------------- SOFT DELETE ----------------------
        public async Task<int> DeleteAsync(string id)
        {
            return await _repo.SoftDeleteAsync(id, _currentUser.UserId);
        }

        // ---------------------- MAPPING ----------------------
        private static RoomSlotDto ToDto(RoomSlot slot)
        {
            return new RoomSlotDto
            {
                Id = slot.Id,
                RoomId = slot.RoomId,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                SlotType = slot.SlotType.ToString(),
                Status = slot.RoomStatus.ToString()
            };
        }
    }
}
