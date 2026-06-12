using LabelForge.Core.Models.System;

namespace LabelForge.Core.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string? module = null, string? entityType = null, string? entityId = null, string? oldValue = null, string? newValue = null);
    Task<IEnumerable<AuditLog>> GetLogsAsync(DateTime? from = null, DateTime? to = null, string? action = null, string? module = null, Guid? userId = null);
}