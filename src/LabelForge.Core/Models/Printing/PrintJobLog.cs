namespace LabelForge.Core.Models.Printing;

public class PrintJobLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PrintJobId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public PrintJob? PrintJob { get; set; }
}