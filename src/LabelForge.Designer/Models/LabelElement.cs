// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public enum ElementType
{
    Text,
    Rectangle,
    RoundedRectangle,
    Circle,
    Ellipse,
    Triangle,
    Line,
    Image,
    Barcode,
    QRCode,
    DateTime,
    Counter,
    DatabaseField,
    RFID
}

public enum OpacityMode
{
    Solid,
    SemiTransparent,
    Custom
}

public class LabelElement : INotifyPropertyChanged
{
    private Guid _id = Guid.NewGuid();
    private string _name = "";
    private ElementType _type;
    private double _x;
    private double _y;
    private double _width;
    private double _height;
    private double _rotation;
    private bool _isSelected;
    private bool _isFlippedHorizontal;
    private bool _isFlippedVertical;
    private bool _isVisible = true;
    private bool _isLocked;
    private Color _backgroundColor = Colors.Transparent;
    private Color _borderColor = Colors.Black;
    private double _borderWidth = 1;
    private double _opacity = 1.0;
    private string _borderStyle = "Solid";
    private double _cornerRadius;
    private double _paddingLeft;
    private double _paddingRight;
    private double _paddingTop;
    private double _paddingBottom;
    private string _dataBinding = "";

    public Guid Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
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

    public bool IsFlippedHorizontal
    {
        get => _isFlippedHorizontal;
        set { _isFlippedHorizontal = value; OnPropertyChanged(); }
    }

    public bool IsFlippedVertical
    {
        get => _isFlippedVertical;
        set { _isFlippedVertical = value; OnPropertyChanged(); }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set { _isVisible = value; OnPropertyChanged(); }
    }

    public bool IsLocked
    {
        get => _isLocked;
        set { _isLocked = value; OnPropertyChanged(); }
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

    public double Opacity
    {
        get => _opacity;
        set { _opacity = value; OnPropertyChanged(); }
    }

    public string BorderStyle
    {
        get => _borderStyle;
        set { _borderStyle = value; OnPropertyChanged(); }
    }

    public double CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = value; OnPropertyChanged(); }
    }

    public double PaddingLeft
    {
        get => _paddingLeft;
        set { _paddingLeft = value; OnPropertyChanged(); }
    }

    public double PaddingRight
    {
        get => _paddingRight;
        set { _paddingRight = value; OnPropertyChanged(); }
    }

    public double PaddingTop
    {
        get => _paddingTop;
        set { _paddingTop = value; OnPropertyChanged(); }
    }

    public double PaddingBottom
    {
        get => _paddingBottom;
        set { _paddingBottom = value; OnPropertyChanged(); }
    }

    public string DataBinding
    {
        get => _dataBinding;
        set { _dataBinding = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}