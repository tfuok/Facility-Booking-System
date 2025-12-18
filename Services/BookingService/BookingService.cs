using Microsoft.Extensions.Logging;
using Repositories.ModelExtensions;
using Repositories.Models;
using Repositories.Models.Enums;
using Repositories.Repo;
using Services.UserService;

namespace Services.BookingService
{
    public class BookingService
    {
        private readonly BookingRepository _repo;
        private readonly RoomSlotRepository _roomSlotRepo;
        private readonly ILogger<BookingService> _logger;
        private readonly ICurrentUserService _currentUser;

        public BookingService(
            BookingRepository repo,
            RoomSlotRepository roomSlotRepo,
            ILogger<BookingService> logger,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _roomSlotRepo = roomSlotRepo;
            _logger = logger;
            _currentUser = currentUser;
        }

        // ---------------------- GET BY ID ----------------------
        public async Task<BookingDto?> GetByIdAsync(string id)
        {
            var booking = await _repo.GetByIdAsync(id);

            if (booking == null || booking.DeletedAt != null)
                return null;

            return ToDto(booking);
        }

        // ---------------------- PAGING ----------------------
        public async Task<PaginationResult<List<BookingDto>>> GetPagingAsync(
            int page,
            int size,
            string? keyword)
        {
            var result = await _repo.GetPagingAsync(page, size, keyword);

            var dtoItems = result.Items.Select(ToDto).ToList();

            return new PaginationResult<List<BookingDto>>
            {
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                Items = dtoItems
            };
        }

        // ---------------------- CREATE ----------------------
        public async Task<int> CreateAsync(BookingCreateRequest request)
        {
            var roomSlot = await _roomSlotRepo.GetByIdAsync(request.RoomSlotId);

            if (roomSlot == null || roomSlot.DeletedAt != null)
                throw new KeyNotFoundException("RoomSlot không tồn tại.");

            if (roomSlot.RoomStatus != Repositories.Models.Enums.RoomSlotStatus.Available)
                throw new InvalidOperationException("RoomSlot không khả dụng để booking.");

            var booking = new Booking
            {
                Id = Guid.NewGuid().ToString(),
                RoomSlotId = request.RoomSlotId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId,
                UserId = _currentUser.UserId,
                RoomId = roomSlot.RoomId,
                Status = BookingStatus.Pending
            };

            //// cập nhật slot thành Unavailable
            //roomSlot.RoomStatus = Repositories.Models.Enums.RoomSlotStatus.Unavailable;
            //roomSlot.UpdatedAt = DateTime.UtcNow;
            //roomSlot.UpdatedBy = _currentUser.UserId;

            //await _roomSlotRepo.UpdateAsync(roomSlot);

            return await _repo.CreateAsync(booking);
        }

        // ---------------------- UPDATE ----------------------
        // ---------------------- UPDATE (USER) ----------------------
        public async Task<int> UpdateAsync(string id, BookingUpdateRequest request)
        {
            var booking = await _repo.GetByIdAsync(id);

            if (booking == null || booking.DeletedAt != null)
                throw new KeyNotFoundException("Booking không tồn tại hoặc đã bị xóa.");

            // chỉ cho phép owner chỉnh
            if (booking.UserId != _currentUser.UserId)
                throw new UnauthorizedAccessException("Không có quyền chỉnh booking này.");

            // kiểm tra slot mới
            var newSlot = await _roomSlotRepo.GetByIdAsync(request.RoomSlotId);
            if (newSlot == null || newSlot.DeletedAt != null)
                throw new KeyNotFoundException("RoomSlot không tồn tại.");

            if (newSlot.RoomStatus != RoomSlotStatus.Available)
                throw new InvalidOperationException("RoomSlot không khả dụng.");

            // trả slot cũ về Available
            booking.RoomSlot!.RoomStatus = RoomSlotStatus.Available;
            await _roomSlotRepo.UpdateAsync(booking.RoomSlot);

            // gán slot mới
            booking.RoomSlotId = request.RoomSlotId;
            newSlot.RoomStatus = RoomSlotStatus.Unavailable;
            await _roomSlotRepo.UpdateAsync(newSlot);

            booking.UpdatedAt = DateTime.UtcNow;
            booking.UpdatedBy = _currentUser.UserId;

            return await _repo.UpdateAsync(booking);
        }

        // ---------------------- UPDATE STATUS (ADMIN) ----------------------
        public async Task<int> UpdateStatusAsync(
            string bookingId,
            BookingStatusUpdateRequest request)
        {
            var booking = await _repo.GetByIdAsync(bookingId);

            if (booking == null || booking.DeletedAt != null)
                throw new KeyNotFoundException("Booking không tồn tại.");

            var roomSlot = booking.RoomSlot
                ?? throw new InvalidOperationException("RoomSlot không tồn tại.");

            // ====== CONFIRM ======
            if (booking.Status == BookingStatus.Pending &&
                request.Status == BookingStatus.Confirmed)
            {
                // Chỉ check đúng slot này
                if (roomSlot.RoomStatus != RoomSlotStatus.Available)
                    throw new InvalidOperationException("Slot đã bị chiếm.");

                roomSlot.RoomStatus = RoomSlotStatus.Unavailable;
                roomSlot.UpdatedAt = DateTime.UtcNow;
                roomSlot.UpdatedBy = _currentUser.UserId;

                await _roomSlotRepo.UpdateAsync(roomSlot);
            }

            // ====== CANCEL ======
            if (request.Status == BookingStatus.Cancelled)
            {
                // Chỉ trả slot nếu slot này đang bị khóa bởi booking này
                if (booking.Status == BookingStatus.Confirmed)
                {
                    roomSlot.RoomStatus = RoomSlotStatus.Available;
                    roomSlot.UpdatedAt = DateTime.UtcNow;
                    roomSlot.UpdatedBy = _currentUser.UserId;

                    await _roomSlotRepo.UpdateAsync(roomSlot);
                }
            }

            booking.Status = request.Status;
            booking.UpdatedAt = DateTime.UtcNow;
            booking.UpdatedBy = _currentUser.UserId;

            return await _repo.UpdateAsync(booking);
        }



        // ---------------------- SOFT DELETE ----------------------
        public async Task<int> DeleteAsync(string id)
        {
            return await _repo.SoftDeleteAsync(id, _currentUser.UserId);
        }

        // ---------------------- MAPPING ----------------------
        private static BookingDto ToDto(Booking booking)
        {
            var room = booking.RoomSlot!.Room!;

            return new BookingDto
            {
                Id = booking.Id,
                RoomNumber = room.RoomNumber,
                RoomName = room.RoomName,
                AreaName = room.Area.Name,
                RoomTypeName = room.RoomType.Name,
                StartTime = booking.RoomSlot.StartTime,
                EndTime = booking.RoomSlot.EndTime,
                SlotType = booking.RoomSlot.SlotType.ToString(),
                SlotStatus = booking.RoomSlot.RoomStatus.ToString(),
                BookingStatus = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt.UtcDateTime,
                CreatedBy = booking.CreatedBy
            };
        }
    }
}
