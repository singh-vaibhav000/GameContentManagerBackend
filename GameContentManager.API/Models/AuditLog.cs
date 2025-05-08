using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GameContentManager.API.Models
{
    public class AuditLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string UserId { get; set; }

        [BsonRequired]
        public string Action { get; set; }

        [BsonRequired]
        public string EntityType { get; set; }

        [BsonRequired]
        public string EntityId { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        [BsonRequired]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Details { get; set; }
    }
} 