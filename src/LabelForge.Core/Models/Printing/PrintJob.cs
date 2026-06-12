using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Printing;

public class PrintJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TemplateId { get; set; }
    public Guid TemplateVersionId { get; set; }
    public Guid PrinterId { get; set; }
    public Guid? RequestedBy { get; set; }
    public RequestedSource RequestedSource { get; set; }
    public int Copies { get; set; } = 1;
    public string? PayloadJson { get; set; }
    public PrintJobStatus Status { get; set; } = PrintJobStatus.Created;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? MachineName { get; set; }
    public Guid? PrintAgentId { get; set; }
    public List<PrintJobLog> Logs { get; set; } = [];
}