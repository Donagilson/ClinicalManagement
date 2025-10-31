using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;

namespace ClinicalManagementSystem2025.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            return await _userRepository.AuthenticateAsync(username, password);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.ToList();
        }

        public async Task AddUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _userRepository.GetAllRolesAsync();
        }
    }
}