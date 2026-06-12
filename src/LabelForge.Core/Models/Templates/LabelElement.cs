using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class LabelElement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ElementType Type { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 100;
    public double Height { get; set; } = 40;
    public double Rotation { get; set; }
    public int LayerIndex { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsLocked { get; set; }
    public bool IsSelected { get; set; }
    public double Opacity { get; set; } = 1.0;
    public string BorderColor { get; set; } = "#000000";
    public double BorderWidth { get; set; }
    public string? FillColor { get; set; }
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;
    public string? PrintCondition { get; set; }
    public string? DataBinding { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}