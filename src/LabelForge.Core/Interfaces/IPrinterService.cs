using LabelForge.Core.Enums;
using LabelForge.Core.Models.Printing;

namespace LabelForge.Core.Interfaces;

public interface IPrinterService
{
    Task<IEnumerable<PrinterInfo>> GetPrintersAsync();
    Task<PrinterInfo?> GetDefaultPrinterAsync();
    Task<PrinterInfo?> GetPrinterByIdAsync(Guid id);
    Task<PrinterStatus> GetPrinterStatusAsync(Guid printerId);
    Task PrintAsync(Guid templateId, Guid printerId, PrintOptions options);
    Task PrintRawAsync(Guid printerId, byte[] data);
    Task PrintZplAsync(Guid printerId, string zpl);
    Task PrintEplAsync(Guid printerId, string epl);
    Task CancelJobAsync(Guid jobId);
    Task<IEnumerable<PrintJob>> GetPrintJobsAsync(Guid printerId);
}