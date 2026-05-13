using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class TextElement : LabelElement
{
    private string _text = "Text";
    private string _fontFamily = "Arial";
    private double _fontSize = 12;
    private bool _bold;
    private bool _italic;
    private bool _underline;
    private Color _textColor = Colors.Black;
    private TextAlignment _horizontalAlignment = TextAlignment.Start;
    private TextAlignment _verticalAlignment = TextAlignment.Start;

    public TextElement()
    {
        Type = ElementType.Text;
        Width = 100;
        Height = 30;
    }

    public string Text
    {
        get => _text;
        set { _text = value; OnPropertyChanged(); }
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

    public bool Italic
    {
        get => _italic;
        set { _italic = value; OnPropertyChanged(); }
    }

    public bool Underline
    {
        get => _underline;
        set { _underline = value; OnPropertyChanged(); }
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

    public TextAlignment VerticalAlignment
    {
        get => _verticalAlignment;
        set { _verticalAlignment = value; OnPropertyChanged(); }
    }
}
