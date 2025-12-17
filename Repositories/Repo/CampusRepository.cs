using Microsoft.EntityFrameworkCore;
using Repositories.Basic;
using Repositories.ModelExtensions;
using Repositories.Models;

namespace Repositories.Repo
{
    public class CampusRepository : GenericRepository<Campus>
    {
        public CampusRepository(FacilityBookingDBContext context) : base(context)
        {
        }

        // -------------------- EXISTS --------------------
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Campuses
                .AsNoTracking()
                .AnyAsync(x => x.Id == id && x.DeletedAt == null);
        }

        public async Task<bool> ExistsByNameAsync(string name, string? excludeId = null)
        {
            return await _context.Campuses
                .AsNoTracking()
                .AnyAsync(c =>
                    c.DeletedAt == null &&
                    c.Name.ToLower() == name.ToLower() &&
                    (excludeId == null || c.Id != excludeId)
                );
        }


        // -------------------- CREATE --------------------
        public new async Task<int> CreateAsync(Campus entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Campuses.Add(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- UPDATE --------------------
        public async Task<int> UpdateAsync(Campus entity)
        {
            var existing = await _context.Campuses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Campus hoặc đã bị xóa (ID = {entity.Id}).");

            _context.Campuses.Update(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- BUILD QUERY --------------------
        private IQueryable<Campus> BuildSearchQuery(string? name, string? address)
        {
            name ??= string.Empty;
            address ??= string.Empty;

            return _context.Campuses
                .AsNoTracking()
                .Where(c =>
                    c.DeletedAt == null &&
                    (string.IsNullOrEmpty(name) || EF.Functions.Like(c.Name!, $"%{name}%")) &&
                    (string.IsNullOrEmpty(address) || EF.Functions.Like(c.Address!, $"%{address}%"))
                );
        }

        // -------------------- PAGING --------------------
        public async Task<PaginationResult<List<Campus>>> GetPagingAsync(
            int currentPage,
            int pageSize,
            string? name,
            string? address)
        {
            if (currentPage <= 0) currentPage = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = BuildSearchQuery(name, address);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<List<Campus>>
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
            var existing = await _context.Campuses
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Campus hoặc đã bị xóa (ID = {id}).");

            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = deletedBy;

            _context.Campuses.Update(existing);
            return await _context.SaveChangesAsync();
        }
    }
}
