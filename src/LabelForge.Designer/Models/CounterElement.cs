using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public enum CounterResetMode
{
    Never,
    Daily,
    Monthly,
    PerTemplate,
    Global
}

public class CounterElement : LabelElement
{
    private long _startValue = 1;
    private long _endValue = 999999;
    private long _stepValue = 1;
    private bool _decrement;
    private string _prefix = "";
    private string _suffix = "";
    private int _padding = 4;
    private bool _isAlphanumeric;
    private CounterResetMode _resetMode = CounterResetMode.Never;
    private string _fontFamily = "Arial";
    private double _fontSize = 12;
    private bool _bold;
    private Color _textColor = Colors.Black;

    public CounterElement()
    {
        Type = ElementType.Counter;
        Name = "Counter";
        Width = 80;
        Height = 25;
    }

    public long StartValue
    {
        get => _startValue;
        set { _startValue = value; OnPropertyChanged(); }
    }

    public long EndValue
    {
        get => _endValue;
        set { _endValue = value; OnPropertyChanged(); }
    }

    public long StepValue
    {
        get => _stepValue;
        set { _stepValue = value; OnPropertyChanged(); }
    }

    public bool Decrement
    {
        get => _decrement;
        set { _decrement = value; OnPropertyChanged(); }
    }

    public string Prefix
    {
        get => _prefix;
        set { _prefix = value; OnPropertyChanged(); }
    }

    public string Suffix
    {
        get => _suffix;
        set { _suffix = value; OnPropertyChanged(); }
    }

    public int Padding
    {
        get => _padding;
        set { _padding = value; OnPropertyChanged(); }
    }

    public bool IsAlphanumeric
    {
        get => _isAlphanumeric;
        set { _isAlphanumeric = value; OnPropertyChanged(); }
    }

    public CounterResetMode ResetMode
    {
        get => _resetMode;
        set { _resetMode = value; OnPropertyChanged(); }
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
}