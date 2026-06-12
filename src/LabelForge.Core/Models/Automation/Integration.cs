using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Automation;

public class Integration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public TriggerType TriggerType { get; set; }
    public string? Description { get; set; }
    public Guid TemplateId { get; set; }
    public Guid? PrinterId { get; set; }
    public string? Configuration { get; set; }
    public string? FieldMapping { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public List<IntegrationTrigger> Triggers { get; set; } = [];
    public List<IntegrationLog> Logs { get; set; } = [];
}