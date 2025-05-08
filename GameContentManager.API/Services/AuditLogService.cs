using GameContentManager.API.Data;
using GameContentManager.API.Models;
using MongoDB.Driver;

namespace GameContentManager.API.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IMongoCollection<AuditLog> _auditLogs;
        
        public AuditLogService(MongoDbContext context)
        {
           
            _auditLogs = context.AuditLogs;
        }
        public async Task LogActionAsync(string userId, string action, string entityId, string entityType, string details,string oldDescription,string Description)
        {
            
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityId = entityId,
                EntityType = entityType,
                Details = details,
                OldValue = oldDescription,
                NewValue=Description,
                Timestamp = DateTime.UtcNow
            };

            await _auditLogs.InsertOneAsync(auditLog);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string entityId = null)
        {
            var filter = entityId == null ? 
                Builders<AuditLog>.Filter.Empty :
                Builders<AuditLog>.Filter.Eq(x => x.EntityId, entityId);

            return await _auditLogs.Find(filter)
                .SortByDescending(x => x.Timestamp)
                .ToListAsync();
        }
    }
} 