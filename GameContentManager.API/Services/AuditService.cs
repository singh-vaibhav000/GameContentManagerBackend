using System.Text.Json;
using System.Threading.Tasks;
using GameContentManager.API.Data;
using GameContentManager.API.Models;
using MongoDB.Driver;

namespace GameContentManager.API.Services
{
    public interface IAuditService
    {
        Task LogCreate(string entityType, string entityId, object entity);
        Task LogUpdate(string entityType, string entityId, object entity);
        Task LogDelete(string entityType, string entityId);
    }

    public class AuditService : IAuditService
    {
        private readonly MongoDbContext _context;

        public AuditService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task LogCreate(string entityType, string entityId, object entity)
        {
            var log = new AuditLog
            {
                Action = "Create",
                EntityType = entityType,
                EntityId = entityId,
                NewValue = JsonSerializer.Serialize(entity)
            };

            await _context.AuditLogs.InsertOneAsync(log);
        }

        public async Task LogUpdate(string entityType, string entityId, object entity)
        {
            var log = new AuditLog
            {
                Action = "Update",
                EntityType = entityType,
                EntityId = entityId,
                NewValue = JsonSerializer.Serialize(entity)
            };

            await _context.AuditLogs.InsertOneAsync(log);
        }

        public async Task LogDelete(string entityType, string entityId)
        {
            var log = new AuditLog
            {
                Action = "Delete",
                EntityType = entityType,
                EntityId = entityId
            };

            await _context.AuditLogs.InsertOneAsync(log);
        }
    }
} 