using Microsoft.EntityFrameworkCore;
using Repositories.Basic;
using Repositories.ModelExtensions;
using Repositories.Models;

namespace Repositories.Repo
{
    public class BookingRepository : GenericRepository<Booking>
    {
        public BookingRepository(FacilityBookingDBContext context) : base(context)
        {
        }

        // -------------------- EXISTS --------------------
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Bookings
                .AsNoTracking()
                .AnyAsync(x => x.Id == id && x.DeletedAt == null);
        }

        // -------------------- CREATE --------------------
        public new async Task<int> CreateAsync(Booking entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Bookings.Add(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- UPDATE --------------------
        public async Task<int> UpdateAsync(Booking entity)
        {
            var existing = await _context.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Booking (ID = {entity.Id}).");

            _context.Bookings.Update(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- GET BY ID (FULL INCLUDE) --------------------
        public override async Task<Booking?> GetByIdAsync(string id)
        {
            return await _context.Bookings
                .Include(b => b.RoomSlot)
                    .ThenInclude(rs => rs.Room)
                        .ThenInclude(r => r.Area)
                .Include(b => b.RoomSlot)
                    .ThenInclude(rs => rs.Room)
                        .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);
        }

        // -------------------- BUILD SEARCH QUERY --------------------
        private IQueryable<Booking> BuildSearchQuery(string? keyword)
        {
            keyword = keyword?.Trim() ?? string.Empty;

            var query = _context.Bookings
                .AsNoTracking()
                .Include(b => b.RoomSlot)
                    .ThenInclude(rs => rs.Room)
                        .ThenInclude(r => r.Area)
                .Include(b => b.RoomSlot)
                    .ThenInclude(rs => rs.Room)
                        .ThenInclude(r => r.RoomType)
                .Where(b => b.DeletedAt == null);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b =>
                    EF.Functions.Like(b.RoomSlot!.Room!.RoomNumber!, $"%{keyword}%")
                    || EF.Functions.Like(b.RoomSlot.Room.RoomName!, $"%{keyword}%")
                    || EF.Functions.Like(b.RoomSlot.Room.Area!.Name!, $"%{keyword}%")
                    || EF.Functions.Like(b.RoomSlot.Room.RoomType!.Name!, $"%{keyword}%")
                    || b.RoomSlot.Room.Floor.ToString().Contains(keyword)
                    || b.RoomSlot.Room.Capacity.ToString().Contains(keyword)
                );
            }

            return query;
        }

        // -------------------- PAGING --------------------
        public async Task<PaginationResult<List<Booking>>> GetPagingAsync(
            int currentPage,
            int pageSize,
            string? keyword)
        {
            if (currentPage <= 0) currentPage = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = BuildSearchQuery(keyword);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<List<Booking>>
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
            var existing = await _context.Bookings
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Booking (ID = {id}).");

            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = deletedBy;

            _context.Bookings.Update(existing);
            return await _context.SaveChangesAsync();
        }
    }
}
