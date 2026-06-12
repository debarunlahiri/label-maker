using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Printing;

public class PrinterInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? PrinterType { get; set; }
    public ConnectionType ConnectionType { get; set; }
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public string MachineName { get; set; } = Environment.MachineName;
    public string? DriverName { get; set; }
    public int Dpi { get; set; } = 203;
    public PrinterStatus Status { get; set; } = PrinterStatus.Unknown;
    public string? Location { get; set; }
    public string? Department { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastSeen { get; set; }
    public string? SupportedSizes { get; set; }
    public PrinterLanguage? PrinterLanguage { get; set; }
}