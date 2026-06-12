namespace LabelForge.Core.Models.Automation;

public class IntegrationTrigger
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IntegrationId { get; set; }
    public string TriggerType { get; set; } = string.Empty;
    public string? Configuration { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Integration Integration { get; set; } = null!;
}