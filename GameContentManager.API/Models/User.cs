using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameContentManager.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRequired]
        public string Email { get; set; } = string.Empty;

        [BsonRequired]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonRequired]
        public List<UserRole> Roles { get; set; } = new List<UserRole>();

        public bool MustChangePassword { get; set; } = false;

        // Audit properties
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    public enum UserRole
    {
        Admin,
        Writer,
        Reader
    }
} 