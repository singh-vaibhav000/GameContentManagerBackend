using GameContentManager.API.Models;

namespace GameContentManager.API.Services
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string userId, string action, string entityId, string entityType, string details,string oldDescription,string description);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string entityId = null);
    }
} 