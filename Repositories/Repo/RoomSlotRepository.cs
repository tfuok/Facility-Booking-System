using Microsoft.EntityFrameworkCore;
using Repositories.Basic;
using Repositories.ModelExtensions;
using Repositories.Models;

namespace Repositories.Repo
{
    public class RoomSlotRepository : GenericRepository<RoomSlot>
    {
        public RoomSlotRepository(FacilityBookingDBContext context) : base(context)
        {
        }

        // -------------------- EXISTS --------------------
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.RoomSlots
                .AsNoTracking()
                .AnyAsync(x => x.Id == id && x.DeletedAt == null);
        }

        // -------------------- CREATE --------------------
        public new async Task<int> CreateAsync(RoomSlot entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.RoomSlots.Add(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- UPDATE --------------------
        public async Task<int> UpdateAsync(RoomSlot entity)
        {
            var existing = await _context.RoomSlots
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy RoomSlot hoặc đã bị xóa (ID = {entity.Id}).");

            _context.RoomSlots.Update(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- BUILD QUERY --------------------
        private IQueryable<RoomSlot> BuildSearchQuery(string? keyword)
        {
            keyword ??= string.Empty;

            return _context.RoomSlots
                .AsNoTracking()
                .Include(x => x.Room)
                .Where(x =>
                    x.DeletedAt == null &&
                    (
                        string.IsNullOrEmpty(keyword)
                        || x.RoomId.Contains(keyword)
                        || x.Room!.RoomNumber.Contains(keyword)
                        || (x.Room.RoomName != null && x.Room.RoomName.Contains(keyword))
                        || x.SlotType.ToString().Contains(keyword)
                        || x.RoomStatus.ToString().Contains(keyword)
                        || x.StartTime.ToString().Contains(keyword)
                        || x.EndTime.ToString().Contains(keyword)
                    )
                );
        }

        // -------------------- PAGING --------------------
        public async Task<PaginationResult<List<RoomSlot>>> GetPagingAsync(
            int currentPage,
            int pageSize,
            string? keyword)
        {
            if (currentPage <= 0) currentPage = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = BuildSearchQuery(keyword);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.StartTime)
                .ThenBy(x => x.EndTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<List<RoomSlot>>
            {
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                CurrentPage = currentPage,
                PageSize = pageSize,
                Items = items
            };
        }

        // -------------------- SOFT DELETE --------------------
        public async Task<int> SoftDeleteAsync(string id, string deletedBy)
        {
            var existing = await _context.RoomSlots
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy RoomSlot hoặc đã bị xóa (ID = {id}).");

            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = deletedBy;

            _context.RoomSlots.Update(existing);
            return await _context.SaveChangesAsync();
        }
    }
}
