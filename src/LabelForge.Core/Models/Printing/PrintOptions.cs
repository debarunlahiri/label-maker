using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Printing;

public class PrintOptions
{
    public int Copies { get; set; } = 1;
    public string? PaperSize { get; set; }
    public PrintOrientation Orientation { get; set; } = PrintOrientation.Portrait;
    public PrintQuality Quality { get; set; } = PrintQuality.Normal;
    public double Darkness { get; set; } = 10;
    public double Speed { get; set; } = 3;
    public bool CutAfterPrint { get; set; } = true;
    public bool PeelOffMode { get; set; }
    public PrinterLanguage OutputLanguage { get; set; } = PrinterLanguage.DriverBased;
    public double MarginTop { get; set; }
    public double MarginBottom { get; set; }
    public double MarginLeft { get; set; }
    public double MarginRight { get; set; }
}

public enum PrintQuality
{
    Draft,
    Normal,
    High
}