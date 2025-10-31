using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}