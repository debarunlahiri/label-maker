using LabelForge.Core.Models.Printing;
using LabelForge.Core.Enums;

namespace LabelForge.PrintAgent.Services;

public class PrintQueueService
{
    private readonly ILogger<PrintQueueService> _logger;
    private readonly List<PrintJob> _queue = [];
    private readonly object _lock = new();

    public PrintQueueService(ILogger<PrintQueueService> logger)
    {
        _logger = logger;
    }

    public Task EnqueueAsync(PrintJob job)
    {
        lock (_lock)
        {
            job.Status = PrintJobStatus.Queued;
            _queue.Add(job);
            _logger.LogInformation("Print job {JobId} enqueued", job.Id);
        }
        return Task.CompletedTask;
    }

    public Task<PrintJob?> GetNextJobAsync()
    {
        lock (_lock)
        {
            var job = _queue
                .Where(j => j.Status == PrintJobStatus.Queued || j.Status == PrintJobStatus.Retrying)
                .OrderBy(j => j.CreatedAt)
                .FirstOrDefault();
            return Task.FromResult(job);
        }
    }

    public async Task ProcessJobAsync(PrintJob job)
    {
        _logger.LogInformation("Processing print job {JobId}", job.Id);

        try
        {
            job.Status = PrintJobStatus.Rendering;
            job.StartedAt = DateTime.UtcNow;

            // TODO: Actual rendering and printing logic
            // 1. Load template
            // 2. Render output
            // 3. Generate printer language (ZPL/EPL/etc.)
            // 4. Send to printer

            await Task.Delay(500); // Simulate printing

            job.Status = PrintJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            _logger.LogInformation("Print job {JobId} completed", job.Id);
        }
        catch (Exception ex)
        {
            job.Status = PrintJobStatus.Failed;
            job.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Print job {JobId} failed", job.Id);
        }
    }

    public Task CancelAsync(Guid jobId)
    {
        lock (_lock)
        {
            var job = _queue.FirstOrDefault(j => j.Id == jobId);
            if (job != null)
            {
                job.Status = PrintJobStatus.Cancelled;
                job.CompletedAt = DateTime.UtcNow;
            }
        }
        return Task.CompletedTask;
    }

    public Task RetryAsync(Guid jobId)
    {
        lock (_lock)
        {
            var job = _queue.FirstOrDefault(j => j.Id == jobId);
            if (job != null && job.Status == PrintJobStatus.Failed)
            {
                job.Status = PrintJobStatus.Retrying;
                job.ErrorMessage = null;
            }
        }
        return Task.CompletedTask;
    }

    public Task ClearCompletedAsync()
    {
        lock (_lock)
        {
            _queue.RemoveAll(j => j.Status == PrintJobStatus.Completed || j.Status == PrintJobStatus.Cancelled);
        }
        return Task.CompletedTask;
    }
}