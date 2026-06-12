using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Automation;

public class ScheduledJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid TemplateId { get; set; }
    public Guid? PrinterId { get; set; }
    public string? DataSourceId { get; set; }
    public int Copies { get; set; } = 1;
    public ScheduleType ScheduleType { get; set; }
    public string? CronExpression { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TimeZone { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? RetryPolicy { get; set; }
    public string? NotificationSettings { get; set; }
    public string? PayloadJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
}