// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

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
    private bool _strikethrough;
    private Color _textColor = Colors.Black;
    private TextAlignment _horizontalAlignment = TextAlignment.Start;
    private TextAlignment _verticalAlignment = TextAlignment.Start;
    private double _lineSpacing = 1.0;
    private double _characterSpacing;
    private string _textWrapping = "Wrap";
    private string _textCase = "None";
    private double _textOutlineWidth;
    private Color _textOutlineColor = Colors.Transparent;

    public TextElement()
    {
        Type = ElementType.Text;
        Name = "Text";
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

    public bool Strikethrough
    {
        get => _strikethrough;
        set { _strikethrough = value; OnPropertyChanged(); }
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

    public double LineSpacing
    {
        get => _lineSpacing;
        set { _lineSpacing = value; OnPropertyChanged(); }
    }

    public double CharacterSpacing
    {
        get => _characterSpacing;
        set { _characterSpacing = value; OnPropertyChanged(); }
    }

    public string TextWrapping
    {
        get => _textWrapping;
        set { _textWrapping = value; OnPropertyChanged(); }
    }

    public string TextCase
    {
        get => _textCase;
        set { _textCase = value; OnPropertyChanged(); }
    }

    public double TextOutlineWidth
    {
        get => _textOutlineWidth;
        set { _textOutlineWidth = value; OnPropertyChanged(); }
    }

    public Color TextOutlineColor
    {
        get => _textOutlineColor;
        set { _textOutlineColor = value; OnPropertyChanged(); }
    }
}