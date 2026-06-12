// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public enum QRCodeErrorCorrection
{
    Low,
    Medium,
    Quartile,
    High
}

public class QRCodeElement : LabelElement
{
    private string _data = "https://example.com";
    private Color _qrColor = Colors.Black;
    private Color _qrBackgroundColor = Colors.White;
    private QRCodeErrorCorrection _errorCorrection = QRCodeErrorCorrection.Medium;
    private double _moduleSize = 4;
    private double _marginSize = 8;

    public QRCodeElement()
    {
        Type = ElementType.QRCode;
        Name = "QR Code";
        Width = 100;
        Height = 100;
    }

    public string Data
    {
        get => _data;
        set { _data = value; OnPropertyChanged(); }
    }

    public Color QRColor
    {
        get => _qrColor;
        set { _qrColor = value; OnPropertyChanged(); }
    }

    public Color QRBackgroundColor
    {
        get => _qrBackgroundColor;
        set { _qrBackgroundColor = value; OnPropertyChanged(); }
    }

    public QRCodeErrorCorrection ErrorCorrection
    {
        get => _errorCorrection;
        set { _errorCorrection = value; OnPropertyChanged(); }
    }

    public double ModuleSize
    {
        get => _moduleSize;
        set { _moduleSize = value; OnPropertyChanged(); }
    }

    public double MarginSize
    {
        get => _marginSize;
        set { _marginSize = value; OnPropertyChanged(); }
    }
}