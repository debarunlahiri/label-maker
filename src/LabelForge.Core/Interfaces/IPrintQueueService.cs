using LabelForge.Core.Models.Printing;

namespace LabelForge.Core.Interfaces;

public interface IPrintQueueService
{
    Task EnqueueAsync(PrintJob job);
    Task<PrintJob?> DequeueAsync(Guid printerId);
    Task CancelAsync(Guid jobId);
    Task RetryAsync(Guid jobId);
    Task PauseAsync(Guid jobId);
    Task ResumeAsync(Guid jobId);
    Task PrioritizeAsync(Guid jobId);
    Task ClearCompletedAsync(Guid printerId);
    Task<IEnumerable<PrintJob>> GetPendingJobsAsync(Guid printerId);
}