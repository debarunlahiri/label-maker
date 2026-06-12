using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class TextElement : LabelElement
{
    public string Text { get; set; } = "Text";
    public string FontFamily { get; set; } = "Arial";
    public double FontSize { get; set; } = 12;
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Underline { get; set; }
    public bool Strikethrough { get; set; }
    public string TextColor { get; set; } = "#000000";
    public string? BackgroundColor { get; set; }
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
    public double LineSpacing { get; set; } = 1.0;
    public double CharacterSpacing { get; set; }
    public TextWrapping TextWrapping { get; set; } = TextWrapping.WordWrap;
    public bool AutoShrink { get; set; }
    public bool AutoFit { get; set; }
    public int MaxLines { get; set; }
    public string? TextDirection { get; set; }
    public bool IsStatic { get; set; } = true;
    public string? DataSourceBinding { get; set; }
    public double TextOutlineWidth { get; set; }
    public string? TextOutlineColor { get; set; }

    public TextElement()
    {
        Type = ElementType.Text;
        Width = 150;
        Height = 30;
    }
}