using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Automation;

public class IntegrationLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IntegrationId { get; set; }
    public string TriggerType { get; set; } = string.Empty;
    public string? InputData { get; set; }
    public string? MappedData { get; set; }
    public string? OutputResult { get; set; }
    public IntegrationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    public Integration Integration { get; set; } = null!;
}