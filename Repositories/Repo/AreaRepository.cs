using Microsoft.EntityFrameworkCore;
using Repositories.Basic;
using Repositories.ModelExtensions;
using Repositories.Models;

namespace Repositories.Repo
{
    public class AreaRepository : GenericRepository<Area>
    {
        public AreaRepository(FacilityBookingDBContext context) : base(context)
        {
        }

        // -------------------- EXISTS --------------------
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Areas
                .AsNoTracking()
                .AnyAsync(x => x.Id == id && x.DeletedAt == null);
        }

        public async Task<bool> ExistsByNameInCampusAsync(
            string campusId,
            string name,
            string? excludeId = null)
        {
            return await _context.Areas
                .AsNoTracking()
                .AnyAsync(a =>
                    a.DeletedAt == null &&
                    a.CampusId == campusId &&
                    a.Name.ToLower() == name.ToLower() &&
                    (excludeId == null || a.Id != excludeId)
                );
        }


        // -------------------- CREATE --------------------
        public new async Task<int> CreateAsync(Area entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Areas.Add(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- UPDATE --------------------
        public async Task<int> UpdateAsync(Area entity)
        {
            var existing = await _context.Areas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Area hoặc đã bị xóa (ID = {entity.Id}).");

            _context.Areas.Update(entity);
            return await _context.SaveChangesAsync();
        }

        // -------------------- BUILD QUERY --------------------
        private IQueryable<Area> BuildSearchQuery(
            string? name,
            string? campusId,
            string? managerId)
        {
            name ??= string.Empty;

            var query = _context.Areas
                .AsNoTracking()
                .Where(a => a.DeletedAt == null);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(a => EF.Functions.Like(a.Name!, $"%{name}%"));

            if (!string.IsNullOrEmpty(campusId))
                query = query.Where(a => a.CampusId == campusId);

            if (!string.IsNullOrEmpty(managerId))
                query = query.Where(a => a.ManagerId == managerId);

            return query;
        }

        // -------------------- PAGING --------------------
        public async Task<PaginationResult<List<Area>>> GetPagingAsync(
            int currentPage,
            int pageSize,
            string? name,
            string? campusId,
            string? managerId)
        {
            if (currentPage <= 0) currentPage = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = BuildSearchQuery(name, campusId, managerId);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.Name)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<List<Area>>
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
            var existing = await _context.Areas
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy Area hoặc đã bị xóa (ID = {id}).");

            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = deletedBy;

            _context.Areas.Update(existing);
            return await _context.SaveChangesAsync();
        }
    }
}
