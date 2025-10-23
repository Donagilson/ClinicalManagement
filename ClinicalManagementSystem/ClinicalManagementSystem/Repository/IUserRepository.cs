using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int userId);
        Task ResetPasswordAsync(int userId, string newPassword);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<User>> GetDoctorsAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        Task<User?> AuthenticateAsync(string username, string password);
    }
}
