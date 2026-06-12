// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

namespace LabelMaker.Services.Printing;

public class PrinterInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Port { get; set; } = "";
    public string Driver { get; set; } = "";
    public bool IsDefault { get; set; }
    public bool IsNetwork { get; set; }
    public PrinterStatus Status { get; set; } = PrinterStatus.Unknown;
    public PrinterType Type { get; set; } = PrinterType.Standard;
    public string Location { get; set; } = "";
    public string Comment { get; set; } = "";
    public List<string> SupportedPaperSizes { get; set; } = new();
    public bool IsLabelPrinter { get; set; }
    public string LabelPrinterLanguage { get; set; } = "ZPL"; // ZPL, EPL, CPCL, DPL
}

public enum PrinterStatus
{
    Unknown,
    Idle,
    Printing,
    Paused,
    Error,
    Offline,
    PaperJam,
    OutOfPaper,
    DoorOpen
}

public enum PrinterType
{
    Standard,
    Label,
    Receipt,
    Card
}

public class PrintJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string PrinterName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalPages { get; set; } = 1;
    public int PrintedPages { get; set; } = 0;
    public PrintJobStatus Status { get; set; } = PrintJobStatus.Pending;
    public string ErrorMessage { get; set; } = "";
}

public enum PrintJobStatus
{
    Pending,
    Spooling,
    Printing,
    Completed,
    Failed,
    Cancelled
}

public class PrintOptions
{
    public string PrinterId { get; set; } = "";
    public int Copies { get; set; } = 1;
    public string PaperSize { get; set; } = "4x6"; // For label printers
    public PrintOrientation Orientation { get; set; } = PrintOrientation.Portrait;
    public PrintQuality Quality { get; set; } = PrintQuality.Normal;
    public double Darkness { get; set; } = 15; // For label printers 0-30
    public int PrintSpeed { get; set; } = 4; // For label printers 1-10
    public bool CutAfterPrint { get; set; } = false;
    public bool PeelOff { get; set; } = false;
    public int LabelWidthMm { get; set; } = 100;
    public int LabelHeightMm { get; set; } = 150;
    public string LabelPrinterLanguage { get; set; } = "ZPL";
}

public enum PrintOrientation
{
    Portrait,
    Landscape
}

public enum PrintQuality
{
    Draft,
    Normal,
    High
}

public interface IPrinterService
{
    Task<List<PrinterInfo>> GetPrintersAsync();
    Task<PrinterInfo?> GetDefaultPrinterAsync();
    Task<bool> PrintAsync(LabelMaker.Models.LabelTemplate template, PrintOptions options);
    Task<bool> PrintRawAsync(byte[] data, PrintOptions options);
    Task<bool> PrintZplAsync(string zplData, PrintOptions options);
    Task<bool> PrintEplAsync(string eplData, PrintOptions options);
    Task<bool> PrintHtmlAsync(string html, PrintOptions options);
    Task<List<PrintJob>> GetPrintJobsAsync();
    Task<bool> CancelJobAsync(Guid jobId);
    Task<bool> CheckPrinterStatusAsync(string printerId);
}

public interface ILabelPrinterGenerator
{
    string GenerateZpl(LabelMaker.Models.LabelTemplate template, PrintOptions options);
    string GenerateEpl(LabelMaker.Models.LabelTemplate template, PrintOptions options);
    string GenerateCpcl(LabelMaker.Models.LabelTemplate template, PrintOptions options);
}
