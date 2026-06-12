// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace LabelMaker.Services.Printing;

public class PrinterManager : IPrinterService
{
    private readonly IPrinterService _platformService;
    private readonly LabelPrinterGenerator _labelGenerator;
    private readonly List<PrintJob> _jobs = new();
    
    public PrinterManager()
    {
        _labelGenerator = new LabelPrinterGenerator();
        
        // Use platform-specific implementation
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _platformService = new WindowsPrinterService();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _platformService = new MacPrinterService();
        }
        else
        {
            _platformService = new FallbackPrinterService();
        }
    }

    public Task<List<PrinterInfo>> GetPrintersAsync()
    {
        return _platformService.GetPrintersAsync();
    }

    public Task<PrinterInfo?> GetDefaultPrinterAsync()
    {
        return _platformService.GetDefaultPrinterAsync();
    }

    public async Task<bool> PrintAsync(LabelMaker.Models.LabelTemplate template, PrintOptions options)
    {
        var job = CreateJob(options.PrinterId);
        
        try
        {
            if (IsLabelPrinter(options))
            {
                // Use label printer language
                string labelData = options.LabelPrinterLanguage.ToUpper() switch
                {
                    "ZPL" => _labelGenerator.GenerateZpl(template, options),
                    "EPL" => _labelGenerator.GenerateEpl(template, options),
                    "CPCL" => _labelGenerator.GenerateCpcl(template, options),
                    _ => _labelGenerator.GenerateZpl(template, options)
                };
                
                return await PrintRawAsync(Encoding.UTF8.GetBytes(labelData), options);
            }
            else
            {
                // Use standard printing
                return await _platformService.PrintAsync(template, options);
            }
        }
        catch (Exception ex)
        {
            job.Status = PrintJobStatus.Failed;
            job.ErrorMessage = ex.Message;
            return false;
        }
    }

    public Task<bool> PrintRawAsync(byte[] data, PrintOptions options)
    {
        return _platformService.PrintRawAsync(data, options);
    }

    public Task<bool> PrintZplAsync(string zplData, PrintOptions options)
    {
        return _platformService.PrintRawAsync(Encoding.UTF8.GetBytes(zplData), options);
    }

    public Task<bool> PrintEplAsync(string eplData, PrintOptions options)
    {
        return _platformService.PrintRawAsync(Encoding.UTF8.GetBytes(eplData), options);
    }

    public Task<bool> PrintHtmlAsync(string html, PrintOptions options)
    {
        return _platformService.PrintHtmlAsync(html, options);
    }

    public Task<List<PrintJob>> GetPrintJobsAsync()
    {
        return Task.FromResult(_jobs.ToList());
    }

    public Task<bool> CancelJobAsync(Guid jobId)
    {
        var job = _jobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            job.Status = PrintJobStatus.Cancelled;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> CheckPrinterStatusAsync(string printerId)
    {
        return _platformService.CheckPrinterStatusAsync(printerId);
    }

    private bool IsLabelPrinter(PrintOptions options)
    {
        return !string.IsNullOrEmpty(options.LabelPrinterLanguage) && 
               options.LabelPrinterLanguage.ToUpper() is "ZPL" or "EPL" or "CPCL" or "DPL";
    }

    private PrintJob CreateJob(string printerName)
    {
        var job = new PrintJob
        {
            Name = $"Label Print {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            PrinterName = printerName,
            Status = PrintJobStatus.Pending
        };
        _jobs.Add(job);
        return job;
    }
}
