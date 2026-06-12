using LabelForge.Core.Models.Automation;

namespace LabelForge.Core.Interfaces;

public interface IIntegrationService
{
    Task<Integration> CreateAsync(Integration integration);
    Task<Integration?> GetByIdAsync(Guid id);
    Task<IEnumerable<Integration>> GetAllAsync();
    Task<Integration> UpdateAsync(Integration integration);
    Task DeleteAsync(Guid id);
    Task<IntegrationLog> ExecuteAsync(Guid integrationId, string inputData);
    Task<IEnumerable<IntegrationLog>> GetLogsAsync(Guid integrationId);
}