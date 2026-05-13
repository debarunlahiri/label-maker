using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public enum ElementType
{
    Text,
    Rectangle,
    Circle,
    Line,
    Image,
    Barcode,
    QRCode
}

public class LabelElement : INotifyPropertyChanged
{
    private Guid _id = Guid.NewGuid();
    private ElementType _type;
    private double _x;
    private double _y;
    private double _width;
    private double _height;
    private double _rotation;
    private bool _isSelected;
    private Color _backgroundColor = Colors.Transparent;
    private Color _borderColor = Colors.Black;
    private double _borderWidth = 1;

    public Guid Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }

    public ElementType Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(); }
    }

    public double X
    {
        get => _x;
        set { _x = value; OnPropertyChanged(); }
    }

    public double Y
    {
        get => _y;
        set { _y = value; OnPropertyChanged(); }
    }

    public double Width
    {
        get => _width;
        set { _width = value; OnPropertyChanged(); }
    }

    public double Height
    {
        get => _height;
        set { _height = value; OnPropertyChanged(); }
    }

    public double Rotation
    {
        get => _rotation;
        set { _rotation = value; OnPropertyChanged(); }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    public Color BackgroundColor
    {
        get => _backgroundColor;
        set { _backgroundColor = value; OnPropertyChanged(); }
    }

    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; OnPropertyChanged(); }
    }

    public double BorderWidth
    {
        get => _borderWidth;
        set { _borderWidth = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
