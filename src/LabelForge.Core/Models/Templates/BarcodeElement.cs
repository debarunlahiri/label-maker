using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class BarcodeElement : LabelElement
{
    public string Data { get; set; } = string.Empty;
    public BarcodeType BarcodeType { get; set; } = BarcodeType.Code128;
    public bool ShowHumanReadableText { get; set; } = true;
    public string BarcodeColor { get; set; } = "#000000";
    public string? BarBackgroundColor { get; set; }
    public double ModuleWidth { get; set; } = 2;
    public double BarHeight { get; set; } = 80;
    public double QuietZone { get; set; } = 10;
    public HorizontalAlignment HumanReadablePosition { get; set; } = HorizontalAlignment.Center;
    public string HumanReadableFontFamily { get; set; } = "Arial";
    public double HumanReadableFontSize { get; set; } = 10;
    public bool IncludeChecksum { get; set; } = true;
    public double ChecksumOffset { get; set; }
    public string? ValidationRule { get; set; }

    public BarcodeElement()
    {
        Type = ElementType.Barcode;
        Width = 200;
        Height = 100;
    }
}