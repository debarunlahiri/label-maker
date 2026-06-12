// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class BarcodeElement : LabelElement
{
    private string _data = "123456789";
    private string _barcodeType = "CODE128";
    private bool _showText = true;
    private Color _barcodeColor = Colors.Black;
    private double _barWidth = 1.0;
    private double _barHeight = 50;
    private double _quietZone = 10;
    private string _textPosition = "Bottom";
    private string _textFontFamily = "Arial";
    private double _textFontSize = 10;
    private double _checksumOffset;
    private bool _includeChecksum;

    public BarcodeElement()
    {
        Type = ElementType.Barcode;
        Name = "Barcode";
        Width = 150;
        Height = 80;
    }

    public string Data
    {
        get => _data;
        set { _data = value; OnPropertyChanged(); }
    }

    public string BarcodeType
    {
        get => _barcodeType;
        set { _barcodeType = value; OnPropertyChanged(); }
    }

    public bool ShowText
    {
        get => _showText;
        set { _showText = value; OnPropertyChanged(); }
    }

    public Color BarcodeColor
    {
        get => _barcodeColor;
        set { _barcodeColor = value; OnPropertyChanged(); }
    }

    public double BarWidth
    {
        get => _barWidth;
        set { _barWidth = value; OnPropertyChanged(); }
    }

    public double BarHeight
    {
        get => _barHeight;
        set { _barHeight = value; OnPropertyChanged(); }
    }

    public double QuietZone
    {
        get => _quietZone;
        set { _quietZone = value; OnPropertyChanged(); }
    }

    public string TextPosition
    {
        get => _textPosition;
        set { _textPosition = value; OnPropertyChanged(); }
    }

    public string TextFontFamily
    {
        get => _textFontFamily;
        set { _textFontFamily = value; OnPropertyChanged(); }
    }

    public double TextFontSize
    {
        get => _textFontSize;
        set { _textFontSize = value; OnPropertyChanged(); }
    }

    public double ChecksumOffset
    {
        get => _checksumOffset;
        set { _checksumOffset = value; OnPropertyChanged(); }
    }

    public bool IncludeChecksum
    {
        get => _includeChecksum;
        set { _includeChecksum = value; OnPropertyChanged(); }
    }
}