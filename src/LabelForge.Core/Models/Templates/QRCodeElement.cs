using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class QRCodeElement : LabelElement
{
    public string Data { get; set; } = string.Empty;
    public QRCodeErrorCorrection ErrorCorrection { get; set; } = QRCodeErrorCorrection.Medium;
    public string QRColor { get; set; } = "#000000";
    public string QRBackgroundColor { get; set; } = "#FFFFFF";
    public int ModuleSize { get; set; } = 4;
    public int MarginSize { get; set; } = 8;

    public QRCodeElement()
    {
        Type = ElementType.QRCode;
        Width = 100;
        Height = 100;
    }
}