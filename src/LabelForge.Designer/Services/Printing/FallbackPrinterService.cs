// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

namespace LabelMaker.Services.Printing;

public class FallbackPrinterService : IPrinterService
{
    public Task<List<PrinterInfo>> GetPrintersAsync()
    {
        return Task.FromResult(new List<PrinterInfo>
        {
            new PrinterInfo
            {
                Id = "Zebra_ZPL",
                Name = "Zebra ZPL (Raw TCP)",
                Port = "9100",
                IsLabelPrinter = true,
                Type = PrinterType.Label,
                LabelPrinterLanguage = "ZPL"
            },
            new PrinterInfo
            {
                Id = "Zebra_EPL",
                Name = "Zebra EPL (Raw TCP)",
                Port = "9100",
                IsLabelPrinter = true,
                Type = PrinterType.Label,
                LabelPrinterLanguage = "EPL"
            },
            new PrinterInfo
            {
                Id = "Generic",
                Name = "Generic Printer",
                IsLabelPrinter = false,
                Type = PrinterType.Standard
            }
        });
    }

    public Task<PrinterInfo?> GetDefaultPrinterAsync()
    {
        return Task.FromResult<PrinterInfo?>(null);
    }

    public async Task<bool> PrintAsync(LabelMaker.Models.LabelTemplate template, PrintOptions options)
    {
        var htmlService = new PrintService();
        var html = await htmlService.GeneratePrintHtmlAsync(template, new PrintSettings());
        return await PrintHtmlAsync(html, options);
    }

    public Task<bool> PrintRawAsync(byte[] data, PrintOptions options)
    {
        return Task.FromResult(false);
    }

    public Task<bool> PrintZplAsync(string zplData, PrintOptions options)
    {
        return PrintRawAsync(System.Text.Encoding.UTF8.GetBytes(zplData), options);
    }

    public Task<bool> PrintEplAsync(string eplData, PrintOptions options)
    {
        return PrintRawAsync(System.Text.Encoding.UTF8.GetBytes(eplData), options);
    }

    public async Task<bool> PrintHtmlAsync(string html, PrintOptions options)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"print_{Guid.NewGuid()}.html");
        await File.WriteAllTextAsync(filePath, html);
        
        try
        {
            await Launcher.OpenAsync(new OpenFileRequest("Print", new ReadOnlyFile(filePath)));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<List<PrintJob>> GetPrintJobsAsync()
    {
        return Task.FromResult(new List<PrintJob>());
    }

    public Task<bool> CancelJobAsync(Guid jobId)
    {
        return Task.FromResult(false);
    }

    public Task<bool> CheckPrinterStatusAsync(string printerId)
    {
        return Task.FromResult(true);
    }
}
