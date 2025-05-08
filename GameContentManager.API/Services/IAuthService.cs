using GameContentManager.API.Models;

namespace GameContentManager.API.Services
{
    public interface IAuthService
    {
        Task<(string Token, User User)> Authenticate(string email, string password);
        Task<User> AuthenticateAsync(string email, string password);
        Task<bool> ChangePassword(string userId, string currentPassword, string newPassword);
        Task<bool> HasRole(string userId, UserRole role);
        string GenerateJwtToken(User user);
        //bool VerifyPassword(string password, string passwordHash);
    }
} 