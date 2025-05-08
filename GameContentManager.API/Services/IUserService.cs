using GameContentManager.API.Models;

namespace GameContentManager.API.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user, string createdBy);
        Task UpdateUserAsync(string id, User user, string updatedBy);
        Task DeleteUserAsync(string id, string deletedBy);
        Task AssignRolesAsync(string userId, List<UserRole> roles, string assignedBy);
        Task ChangePasswordAsync(string userId, string newPassword, string changedBy);
    }
} 