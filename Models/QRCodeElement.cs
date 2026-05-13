using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class QRCodeElement : LabelElement
{
    private string _data = "https://example.com";
    private Color _qrColor = Colors.Black;

    public QRCodeElement()
    {
        Type = ElementType.QRCode;
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
}
