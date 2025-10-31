using System.Data.SqlClient;
using ClinicalManagementSystem2025.Models;
using Microsoft.Extensions.Configuration;

namespace ClinicalManagementSystem2025.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _conn;

        public UserRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("MVCConnectionString");
        }

        // Authenticate user
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            using var conn = new SqlConnection(_conn);
            var sql = @"
                SELECT u.UserId, u.UserName, u.UserPassword, u.FullName, u.Email, u.Phone,
                       u.RoleId, u.IsActive, r.RoleName
                FROM TblUsers u
                JOIN TblRoles r ON u.RoleId = r.RoleId
                WHERE u.UserName = @UserName AND u.UserPassword = @UserPassword AND u.IsActive = 1";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserName", username);
            cmd.Parameters.AddWithValue("@UserPassword", password);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = (int)reader["UserId"],
                    UserName = reader["UserName"].ToString() ?? "",
                    FullName = reader["FullName"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    Phone = reader["Phone"].ToString() ?? "",
                    RoleId = (int)reader["RoleId"],
                    Role = new Role
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString() ?? ""
                    }
                };
            }

            return null;
        }

        // Get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var list = new List<User>();
            using var conn = new SqlConnection(_conn);
            var sql = @"
                SELECT u.UserId, u.UserName, u.FullName, u.Email, u.Phone, u.RoleId, r.RoleName, u.IsActive, u.CreatedDate
                FROM TblUsers u
                JOIN TblRoles r ON u.RoleId = r.RoleId
                ORDER BY u.CreatedDate DESC";

            using var cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    UserId = (int)reader["UserId"],
                    UserName = reader["UserName"].ToString() ?? "",
                    FullName = reader["FullName"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    Phone = reader["Phone"].ToString() ?? "",
                    RoleId = (int)reader["RoleId"],
                    Role = new Role
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString() ?? ""
                    },
                    IsActive = (bool)reader["IsActive"],
                    CreatedDate = (DateTime)reader["CreatedDate"]
                });
            }

            return list;
        }

        // Get user by ID
        public async Task<User?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_conn);
            var sql = @"
                SELECT u.UserId, u.UserName, u.FullName, u.Email, u.Phone, u.RoleId, r.RoleName, u.IsActive
                FROM TblUsers u
                JOIN TblRoles r ON u.RoleId = r.RoleId
                WHERE u.UserId = @UserId";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = (int)reader["UserId"],
                    UserName = reader["UserName"].ToString() ?? "",
                    FullName = reader["FullName"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    Phone = reader["Phone"].ToString() ?? "",
                    RoleId = (int)reader["RoleId"],
                    Role = new Role
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString() ?? ""
                    },
                    IsActive = (bool)reader["IsActive"]
                };
            }

            return null;
        }

        // Add a new user
        public async Task AddUserAsync(User user)
        {
            using var conn = new SqlConnection(_conn);
            var sql = @"
                INSERT INTO TblUsers (UserName, UserPassword, FullName, Email, Phone, RoleId, IsActive, CreatedDate)
                VALUES (@UserName, @UserPassword, @FullName, @Email, @Phone, @RoleId, 1, GETDATE())";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@UserPassword", user.UserPassword ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FullName", user.FullName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // Update user
        public async Task UpdateUserAsync(User user)
        {
            using var conn = new SqlConnection(_conn);
            var sql = @"
                UPDATE TblUsers
                SET UserName = @UserName,
                    FullName = @FullName,
                    Email = @Email,
                    Phone = @Phone,
                    RoleId = @RoleId,
                    IsActive = @IsActive
                WHERE UserId = @UserId";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", user.UserId);
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@FullName", user.FullName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // Delete user (soft delete)
        public async Task DeleteUserAsync(int userId)
        {
            using var conn = new SqlConnection(_conn);
            var sql = "UPDATE TblUsers SET IsActive = 0 WHERE UserId = @UserId";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // Reset password
        public async Task ResetPasswordAsync(int userId, string newPassword)
        {
            using var conn = new SqlConnection(_conn);
            var sql = "UPDATE TblUsers SET UserPassword = @NewPassword WHERE UserId = @UserId";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@NewPassword", newPassword);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // Get all roles
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            var list = new List<Role>();
            using var conn = new SqlConnection(_conn);
            var sql = "SELECT RoleId, RoleName FROM TblRoles ORDER BY RoleName";

            using var cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Role
                {
                    RoleId = (int)reader["RoleId"],
                    RoleName = reader["RoleName"].ToString() ?? ""
                });
            }

            return list;
        }

        public async Task<IEnumerable<User>> GetDoctorsAsync()
        {
            return await GetUsersByRoleAsync("Doctor");
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            var users = new List<User>();
            using var conn = new SqlConnection(_conn);
            const string sql = @"
                SELECT u.UserId, u.UserName, u.FullName, u.Email, u.Phone,
                       u.RoleId, r.RoleName, u.IsActive, u.CreatedDate
                FROM TblUsers u
                INNER JOIN TblRoles r ON u.RoleId = r.RoleId
                WHERE r.RoleName = @RoleName AND u.IsActive = 1
                ORDER BY u.FullName";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@RoleName", roleName);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = (int)reader["UserId"],
                    UserName = reader["UserName"].ToString() ?? "",
                    FullName = reader["FullName"].ToString() ?? "",
                    Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                    Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"].ToString(),
                    RoleId = (int)reader["RoleId"],
                    IsActive = (bool)reader["IsActive"],
                    CreatedDate = (DateTime)reader["CreatedDate"],
                    Role = new Role
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString() ?? ""
                    }
                });
            }

            return users;
        }
    }
}
