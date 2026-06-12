using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public enum DateTimeValueType
{
    CurrentDate,
    CurrentTime,
    CurrentDateTime,
    CustomFormat,
    OffsetDate,
    ExpiryDate
}

public class DateTimeElement : LabelElement
{
    private string _format = "dd-MM-yyyy";
    private DateTimeValueType _valueType = DateTimeValueType.CurrentDate;
    private string _customFormat = "";
    private bool _usePrinterDate;
    private bool _useServerDate;
    private int _offsetDays;
    private string _offsetDirection = "Add";
    private string _fontFamily = "Arial";
    private double _fontSize = 12;
    private bool _bold;
    private Color _textColor = Colors.Black;
    private TextAlignment _horizontalAlignment = TextAlignment.Start;

    public DateTimeElement()
    {
        Type = ElementType.DateTime;
        Name = "DateTime";
        Width = 120;
        Height = 25;
    }

    public string Format
    {
        get => _format;
        set { _format = value; OnPropertyChanged(); }
    }

    public DateTimeValueType ValueType
    {
        get => _valueType;
        set { _valueType = value; OnPropertyChanged(); }
    }

    public string CustomFormat
    {
        get => _customFormat;
        set { _customFormat = value; OnPropertyChanged(); }
    }

    public bool UsePrinterDate
    {
        get => _usePrinterDate;
        set { _usePrinterDate = value; OnPropertyChanged(); }
    }

    public bool UseServerDate
    {
        get => _useServerDate;
        set { _useServerDate = value; OnPropertyChanged(); }
    }

    public int OffsetDays
    {
        get => _offsetDays;
        set { _offsetDays = value; OnPropertyChanged(); }
    }

    public string OffsetDirection
    {
        get => _offsetDirection;
        set { _offsetDirection = value; OnPropertyChanged(); }
    }

    public string FontFamily
    {
        get => _fontFamily;
        set { _fontFamily = value; OnPropertyChanged(); }
    }

    public double FontSize
    {
        get => _fontSize;
        set { _fontSize = value; OnPropertyChanged(); }
    }

    public bool Bold
    {
        get => _bold;
        set { _bold = value; OnPropertyChanged(); }
    }

    public Color TextColor
    {
        get => _textColor;
        set { _textColor = value; OnPropertyChanged(); }
    }

    public TextAlignment HorizontalAlignment
    {
        get => _horizontalAlignment;
        set { _horizontalAlignment = value; OnPropertyChanged(); }
    }
}