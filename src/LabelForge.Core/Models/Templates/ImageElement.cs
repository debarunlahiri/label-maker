using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.Templates;

public class ImageElement : LabelElement
{
    public string? ImagePath { get; set; }
    public ImageSourceType ImageSource { get; set; } = ImageSourceType.Embedded;
    public ImageScalingMode ScalingMode { get; set; } = ImageScalingMode.Fit;
    public bool MaintainAspectRatio { get; set; } = true;
    public double Brightness { get; set; } = 1.0;
    public double Contrast { get; set; } = 1.0;
    public string? TintColor { get; set; }

    public ImageElement()
    {
        Type = ElementType.Image;
        Width = 100;
        Height = 100;
    }
}