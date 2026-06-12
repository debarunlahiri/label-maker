// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class ShapeElement : LabelElement
{
    private bool _filled;
    private Color _fillColor = Colors.Transparent;
    private double _cornerRadius;
    private double _lineDash;
    private double _lineSpacing;
    private string _lineStyle = "Solid";
    private double _startAngle;
    private double _sweepAngle = 360;

    public ShapeElement(ElementType type)
    {
        Type = type;
        Name = type switch
        {
            ElementType.RoundedRectangle => "Rounded Rect",
            ElementType.Ellipse => "Ellipse",
            ElementType.Triangle => "Triangle",
            _ => type.ToString()
        };
        switch (type)
        {
            case ElementType.Rectangle:
                Width = 100; Height = 50; break;
            case ElementType.RoundedRectangle:
                Width = 100; Height = 50; _cornerRadius = 10; break;
            case ElementType.Circle:
                Width = 80; Height = 80; break;
            case ElementType.Ellipse:
                Width = 120; Height = 60; break;
            case ElementType.Triangle:
                Width = 80; Height = 70; break;
            case ElementType.Line:
                Width = 100; Height = 2; break;
        }
    }

    public bool Filled
    {
        get => _filled;
        set { _filled = value; OnPropertyChanged(); }
    }

    public Color FillColor
    {
        get => _fillColor;
        set { _fillColor = value; OnPropertyChanged(); }
    }

    public double CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = value; OnPropertyChanged(); }
    }

    public double LineDash
    {
        get => _lineDash;
        set { _lineDash = value; OnPropertyChanged(); }
    }

    public double LineSpacing
    {
        get => _lineSpacing;
        set { _lineSpacing = value; OnPropertyChanged(); }
    }

    public string LineStyle
    {
        get => _lineStyle;
        set { _lineStyle = value; OnPropertyChanged(); }
    }

    public double StartAngle
    {
        get => _startAngle;
        set { _startAngle = value; OnPropertyChanged(); }
    }

    public double SweepAngle
    {
        get => _sweepAngle;
        set { _sweepAngle = value; OnPropertyChanged(); }
    }
}