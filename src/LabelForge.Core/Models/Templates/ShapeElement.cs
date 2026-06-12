using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class ShapeElement : LabelElement
{
    public ShapeType ShapeType { get; set; } = ShapeType.Rectangle;
    public bool Filled { get; set; } = true;
    public new string FillColor { get; set; } = "#FFFFFF";
    public double CornerRadius { get; set; }
    public string LineDash { get; set; } = string.Empty;
    public double LineSpacing { get; set; }
    public double LineThickness { get; set; } = 1;
    public string LineColor { get; set; } = "#000000";

    public ShapeElement()
    {
        Type = ElementType.Shape;
        Width = 100;
        Height = 60;
    }
}