using Repositories.ModelExtensions;
using Repositories.Models;
using Repositories.Models.Enums;
using Repositories.Repo;

namespace Services.UserService
{
    public class UserService
    {
        private readonly UserRepository _repository;
        public UserService(UserRepository repo)
        {
            _repository = repo;
        }
        public async Task<User> GetUserAccount(string userName, string password)
        {
            return await _repository.GetUserAccount(userName, password);
        }

        public async Task<User> Register(User user)
        {
            // Check email trùng
            var exist = await _repository.GetUserByEmail(user.Email);
            if (exist != null)
                return null;

            return await _repository.Register(user);
        }

        // ---------------- PAGING ----------------
        public async Task<PaginationResult<List<UserDto>>> GetPagingAsync(
            int page,
            int size,
            string? keyword,
            Role? role)
        {
            var result = await _repository.GetPagingAsync(page, size, keyword, role);

            return new PaginationResult<List<UserDto>>
            {
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                Items = result.Items.Select(x => new UserDto
                {
                    Id = x.Id,
                    Email = x.Email,
                    Username = x.Username,
                    PhoneNumber = x.PhoneNumber,
                    Role = x.Role
                }).ToList()
            };
        }
    }
}
