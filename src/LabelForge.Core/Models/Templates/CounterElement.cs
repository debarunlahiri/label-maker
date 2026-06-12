using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class CounterElement : LabelElement
{
    public long StartValue { get; set; } = 1;
    public long EndValue { get; set; } = 999999;
    public long StepValue { get; set; } = 1;
    public bool Decrement { get; set; }
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public int Padding { get; set; } = 4;
    public bool IsAlphanumeric { get; set; }
    public CounterResetMode ResetMode { get; set; } = CounterResetMode.Never;
    public string FontFamily { get; set; } = "Arial";
    public double FontSize { get; set; } = 12;
    public bool Bold { get; set; }
    public string TextColor { get; set; } = "#000000";

    public CounterElement()
    {
        Type = ElementType.Counter;
        Width = 80;
        Height = 25;
    }
}