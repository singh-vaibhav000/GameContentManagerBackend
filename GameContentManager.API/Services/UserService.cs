using System.Security.Cryptography;
using System.Text;
using GameContentManager.API.Data;
using GameContentManager.API.Models;
using MongoDB.Driver;

namespace GameContentManager.API.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IAuditLogService _auditLogService;

        public UserService(MongoDbContext context, IAuditLogService auditLogService)
        {
            _users = context.Users;
            _auditLogService = auditLogService;
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user, string createdBy)
        {
            // Hash the password
            user.PasswordHash = user.PasswordHash;
            user.CreatedAt = DateTime.UtcNow;
            user.CreatedBy = createdBy;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = createdBy;
            user.MustChangePassword = false;

            await _users.InsertOneAsync(user);
            await _auditLogService.LogActionAsync(null,createdBy, "Create", "User", user.Id, $"Created user {user.Email}",null);
            return user;
        }

        public async Task UpdateUserAsync(string id, User user, string updatedBy)
        {
            try
            {
                var existingUser = await GetUserByIdAsync(id);
                if (existingUser == null)
                    throw new Exception("User not found");

                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = updatedBy;

                await _users.ReplaceOneAsync(u => u.Id == id, user);
                //await _auditLogService.LogActionAsync(null,updatedBy, "Update", "User", id, $"Updated user {user.Email}",null);
            }
            catch (Exception ex)
            {
                // Log this
                Console.WriteLine($"Error in UpdateUserAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteUserAsync(string id, string deletedBy)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            await _users.DeleteOneAsync(u => u.Id == id);
            await _auditLogService.LogActionAsync(null,deletedBy, "Delete", "User", id, $"Deleted user {user.Email}", null);
        }

        public async Task AssignRolesAsync(string userId, List<UserRole> roles, string assignedBy)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.Roles = roles;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = assignedBy;

            await _users.ReplaceOneAsync(u => u.Id == userId, user);
            await _auditLogService.LogActionAsync(null, assignedBy, "Update", "User", userId, $"Assigned roles to user {user.Email}", null);
        }

        public async Task ChangePasswordAsync(string userId, string newPassword, string changedBy)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.PasswordHash = HashPassword(newPassword);
            user.MustChangePassword = false;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = changedBy;

            await _users.ReplaceOneAsync(u => u.Id == userId, user);
            await _auditLogService.LogActionAsync(null, changedBy, "Update", "User", userId, $"Changed password for user {user.Email}", null);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 