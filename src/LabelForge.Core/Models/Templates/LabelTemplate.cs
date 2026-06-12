using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class LabelTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Untitled";
    public string? Description { get; set; }
    public double Width { get; set; } = 400;
    public double Height { get; set; } = 300;
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public int Dpi { get; set; } = 203;
    public double MarginTop { get; set; }
    public double MarginBottom { get; set; }
    public double MarginLeft { get; set; }
    public double MarginRight { get; set; }
    public double GridSpacing { get; set; } = 10;
    public bool SnapToGrid { get; set; }
    public UnitType Unit { get; set; } = UnitType.Pixels;
    public PrintOrientation Orientation { get; set; } = PrintOrientation.Portrait;
    public int Rows { get; set; } = 1;
    public int Columns { get; set; } = 1;
    public double HorizontalGap { get; set; }
    public double VerticalGap { get; set; }
    public int Copies { get; set; } = 1;
    public string? PrinterName { get; set; }
    public TemplateStatus Status { get; set; } = TemplateStatus.Draft;
    public Guid? CurrentVersionId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<LabelElement> Elements { get; set; } = [];
    public List<TemplateVersion> Versions { get; set; } = [];
}