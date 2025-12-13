using Microsoft.EntityFrameworkCore;
using Repositories.Basic;
using Repositories.ModelExtensions;
using Repositories.Models;

namespace Repositories.Repo
{
    public class RoomTypeRepository : GenericRepository<RoomType>
    {
        public RoomTypeRepository(FacilityBookingDBContext context) : base(context)
        {
        }

        // -------------------- EXISTS --------------------
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.RoomTypes
                .AsNoTracking()
                .AnyAsync(x => x.Id == id && x.DeletedAt == null);
        }

        // -------------------- CREATE --------------------
        public new async Task<int> CreateAsync(RoomType entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.RoomTypes.Add(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- UPDATE --------------------
        public async Task<int> UpdateAsync(RoomType entity)
        {
            var existing = await _context.RoomTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy RoomType hoặc đã bị xóa (ID = {entity.Id}).");

            _context.RoomTypes.Update(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- BUILD QUERY --------------------
        private IQueryable<RoomType> BuildSearchQuery(string? name)
        {
            name ??= string.Empty;

            return _context.RoomTypes
                .AsNoTracking()
                .Where(c =>
                    c.DeletedAt == null &&
                    (string.IsNullOrEmpty(name) || EF.Functions.Like(c.Name!, $"%{name}%"))
                );
        }

        // -------------------- PAGING --------------------
        public async Task<PaginationResult<List<RoomType>>> GetPagingAsync(
            int currentPage,
            int pageSize,
            string? name)
        {
            if (currentPage <= 0) currentPage = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = BuildSearchQuery(name);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<List<RoomType>>
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
            var existing = await _context.RoomTypes
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Campus hoặc đã bị xóa (ID = {id}).");

            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = deletedBy;

            _context.RoomTypes.Update(existing);
            return await _context.SaveChangesAsync();
        }
    }
}
