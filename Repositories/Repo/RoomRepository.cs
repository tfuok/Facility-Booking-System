using Microsoft.EntityFrameworkCore;
using Repositories.Basic;
using Repositories.ModelExtensions;
using Repositories.Models;

namespace Repositories.Repo
{
    public class RoomRepository : GenericRepository<Room>
    {
        public RoomRepository(FacilityBookingDBContext context) : base(context)
        {
        }

        // -------------------- EXISTS --------------------
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Rooms
                .AsNoTracking()
                .AnyAsync(x => x.Id == id && x.DeletedAt == null);
        }

        // -------------------- CREATE --------------------
        public new async Task<int> CreateAsync(Room entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Rooms.Add(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- UPDATE --------------------
        public async Task<int> UpdateAsync(Room entity)
        {
            var existing = await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Room hoặc đã bị xóa (ID = {entity.Id}).");

            _context.Rooms.Update(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- GET BY ID (INCLUDE) --------------------
        public override async Task<Room?> GetByIdAsync(string id)
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Area)
                .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);
        }

        // -------------------- BUILD SEARCH QUERY --------------------
        private IQueryable<Room> BuildSearchQuery(string? keyword)
        {
            keyword = keyword?.Trim() ?? string.Empty;

            return _context.Rooms
                .AsNoTracking()
                .Include(r => r.RoomType)
                .Include(r => r.Area)
                .Where(r =>
                    r.DeletedAt == null &&
                    (
                        string.IsNullOrEmpty(keyword)
                        || EF.Functions.Like(r.RoomNumber!, $"%{keyword}%")
                        || EF.Functions.Like(r.RoomName!, $"%{keyword}%")
                        || EF.Functions.Like(r.RoomType!.Name!, $"%{keyword}%")
                        || EF.Functions.Like(r.Area!.Name!, $"%{keyword}%")
                        || r.Floor.ToString().Contains(keyword)
                        || r.Capacity.ToString().Contains(keyword)
                    )
                );
        }

        // -------------------- PAGING --------------------
        public async Task<PaginationResult<List<Room>>> GetPagingAsync(
            int currentPage,
            int pageSize,
            string? keyword)
        {
            if (currentPage <= 0) currentPage = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = BuildSearchQuery(keyword);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(r => r.RoomNumber)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<List<Room>>
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
            var existing = await _context.Rooms
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Room hoặc đã bị xóa (ID = {id}).");

            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = deletedBy;

            _context.Rooms.Update(existing);
            return await _context.SaveChangesAsync();
        }
    }
}
