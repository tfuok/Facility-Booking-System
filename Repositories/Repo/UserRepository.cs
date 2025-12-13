using Microsoft.EntityFrameworkCore;
using Repositories.Basic;
using Repositories.Models;

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
    }
}
