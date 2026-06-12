using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class TemplateVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string TemplateJson { get; set; } = string.Empty;
    public RevisionStatus Status { get; set; } = RevisionStatus.Draft;
    public string? ChangeComment { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public LabelTemplate? Template { get; set; }
}