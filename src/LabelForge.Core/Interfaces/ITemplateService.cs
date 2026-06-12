using LabelForge.Core.Models.Templates;

namespace LabelForge.Core.Interfaces;

public interface ITemplateService
{
    Task<LabelTemplate> CreateAsync(LabelTemplate template);
    Task<LabelTemplate?> GetByIdAsync(Guid id);
    Task<IEnumerable<LabelTemplate>> GetAllAsync();
    Task<LabelTemplate> UpdateAsync(LabelTemplate template);
    Task DeleteAsync(Guid id);
    Task<TemplateVersion> SaveVersionAsync(Guid templateId, string changeComment);
    Task<TemplateVersion?> GetVersionAsync(Guid templateId, int versionNumber);
    Task<IEnumerable<TemplateVersion>> GetVersionHistoryAsync(Guid templateId);
    Task<TemplateVersion> SubmitForApprovalAsync(Guid versionId);
    Task<TemplateVersion> ApproveAsync(Guid versionId, Guid approverId);
    Task<TemplateVersion> RejectAsync(Guid versionId, Guid approverId, string reason);
}