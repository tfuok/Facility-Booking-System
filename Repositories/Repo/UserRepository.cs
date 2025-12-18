using Microsoft.EntityFrameworkCore;
using Repositories.Basic;
using Repositories.ModelExtensions;
using Repositories.Models;
using Repositories.Models.Enums;

namespace Repositories.Repo
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(FacilityBookingDBContext context) : base(context)
        {
        }

        public async Task<User> GetUserAccount(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.DeletedAt == null);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null);
        }

        public async Task<User> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<PaginationResult<List<User>>> GetPagingAsync(
            int page,
            int size,
            string? keyword,
            Role? role)
        {
            if (page <= 0) page = 1;
            if (size <= 0) size = 10;

            var query = _context.Users
                .AsNoTracking()
                .Where(x => x.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.Email.Contains(keyword) ||
                    x.Username!.Contains(keyword));
            }

            if (role != null)
            {
                query = query.Where(x => x.Role == role);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Email)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PaginationResult<List<User>>
            {
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)size),
                CurrentPage = page,
                PageSize = size,
                Items = items
            };
        }
    }
}
