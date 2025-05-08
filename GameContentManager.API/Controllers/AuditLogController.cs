using GameContentManager.API.Models;
using GameContentManager.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameContentManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? userId,
            [FromQuery] string? entityType,
            [FromQuery] string? action)
        {
            var logs = await _auditLogService.GetAuditLogsAsync();
            
            // Apply filters
            if (startDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp >= startDate.Value);
            }
            
            if (endDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp <= endDate.Value);
            }
            
            if (!string.IsNullOrEmpty(userId))
            {
                logs = logs.Where(l => l.UserId == userId);
            }
            
            if (!string.IsNullOrEmpty(entityType))
            {
                logs = logs.Where(l => l.EntityType == entityType);
            }
            
            if (!string.IsNullOrEmpty(action))
            {
                logs = logs.Where(l => l.Action == action);
            }

            return Ok(logs);
        }
    }
} 