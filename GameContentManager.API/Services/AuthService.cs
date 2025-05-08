using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GameContentManager.API.Data;
using GameContentManager.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Security.Cryptography;
namespace GameContentManager.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> _users;

        public AuthService(MongoDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _users = context.Users;
        }

        public async Task<(string Token, User User)> Authenticate(string email, string password)
        {
            var user = await AuthenticateAsync(email, password);
            if (user == null)
                return (null,null);

            var token = GenerateJwtToken(user);
            return (token, user);
        }


        public async Task<bool> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return false;

            // Compare current password with stored one (in plain text)
            if (user.PasswordHash != currentPassword)
                return false;

            // Update password and MustChangePassword flag
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, newPassword)
                .Set(u => u.MustChangePassword, true);

            var result = await _users.UpdateOneAsync(u => u.Id == userId, update);
            return result.ModifiedCount > 0;
        }


        public async Task<bool> HasRole(string userId, UserRole role)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            return user?.Roles.Contains(role) ?? false;
        }

        public string GenerateJwtToken(User user)
        {
            // Derive the key exactly as done in Program.cs
            var key = Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:SecretKey"]
                .PadRight(32, '0')
                .Substring(0, 32)
            );

            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Set user claims (no iss/aud here!)
            var claims = new List<Claim>
    {
        new Claim("sub", user.Id),
        new Claim(ClaimTypes.Email, user.Email)
    };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
                return null;

            // For testing, use simple password comparison
            if (user.PasswordHash != password)
                return null;

            return user;
        }
    }
} 