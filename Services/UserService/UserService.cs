using Repositories.Models;
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
    }
}
