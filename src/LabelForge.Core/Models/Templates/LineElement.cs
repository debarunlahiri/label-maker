namespace LabelForge.Core.Models.Templates;

public class LineElement : LabelElement
{
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; } = 100;
    public double EndY { get; set; }
    public double LineThickness { get; set; } = 1;
    public string LineColor { get; set; } = "#000000";
    public string LineStyle { get; set; } = "Solid";
    public bool ArrowStart { get; set; }
    public bool ArrowEnd { get; set; }

    public LineElement()
    {
        Type = Enums.ElementType.Line;
        Width = 100;
        Height = 2;
    }
}