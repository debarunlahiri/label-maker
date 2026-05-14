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

    public BarcodeElement()
    {
        Type = ElementType.Barcode;
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
}
