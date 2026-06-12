using LabelForge.Core.Enums;
using LabelForge.Core.Models.Printing;

namespace LabelForge.Core.Interfaces;

public interface IPrintJobService
{
    Task<PrintJob> CreateAsync(PrintJob job);
    Task<PrintJob?> GetByIdAsync(Guid id);
    Task<IEnumerable<PrintJob>> GetByPrinterAsync(Guid printerId);
    Task<IEnumerable<PrintJob>> GetByTemplateAsync(Guid templateId);
    Task<PrintJob> UpdateStatusAsync(Guid jobId, PrintJobStatus status, string? errorMessage = null);
    Task CancelAsync(Guid jobId);
    Task RetryAsync(Guid jobId);
    Task AddLogAsync(Guid jobId, string action, string? message = null);
}