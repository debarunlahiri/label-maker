using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class DatabaseFieldElement : LabelElement
{
    private string _dataSourceId = "";
    private string _fieldName = "";
    private string _fontFamily = "Arial";
    private double _fontSize = 12;
    private bool _bold;
    private Color _textColor = Colors.Black;
    private TextAlignment _horizontalAlignment = TextAlignment.Start;
    private string _format = "";

    public DatabaseFieldElement()
    {
        Type = ElementType.DatabaseField;
        Name = "DB Field";
        Width = 120;
        Height = 25;
    }

    public string DataSourceId
    {
        get => _dataSourceId;
        set { _dataSourceId = value; OnPropertyChanged(); }
    }

    public string FieldName
    {
        get => _fieldName;
        set { _fieldName = value; OnPropertyChanged(); }
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

    public string Format
    {
        get => _format;
        set { _format = value; OnPropertyChanged(); }
    }
}