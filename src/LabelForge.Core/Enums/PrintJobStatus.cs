namespace LabelForge.Core.Enums;

public enum PrintJobStatus
{
    Created,
    Queued,
    Processing,
    Rendering,
    Sent,
    Printing,
    Completed,
    Failed,
    Cancelled,
    Retrying
}