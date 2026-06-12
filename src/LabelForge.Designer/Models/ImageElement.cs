// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class ImageElement : LabelElement
{
    private string _imagePath = "";
    private string _imageSource = "";
    private double _opacity = 1.0;
    private string _scalingMode = "Uniform";
    private double _brightness;
    private double _contrast;
    private Color _tintColor = Colors.Transparent;
    private bool _maintainAspectRatio = true;

    public ImageElement()
    {
        Type = ElementType.Image;
        Name = "Image";
        Width = 100;
        Height = 100;
    }

    public string ImagePath
    {
        get => _imagePath;
        set { _imagePath = value; OnPropertyChanged(); }
    }

    public string ImageSource
    {
        get => _imageSource;
        set { _imageSource = value; OnPropertyChanged(); }
    }

    public new double Opacity
    {
        get => _opacity;
        set { _opacity = value; OnPropertyChanged(); }
    }

    public string ScalingMode
    {
        get => _scalingMode;
        set { _scalingMode = value; OnPropertyChanged(); }
    }

    public double Brightness
    {
        get => _brightness;
        set { _brightness = value; OnPropertyChanged(); }
    }

    public double Contrast
    {
        get => _contrast;
        set { _contrast = value; OnPropertyChanged(); }
    }

    public Color TintColor
    {
        get => _tintColor;
        set { _tintColor = value; OnPropertyChanged(); }
    }

    public bool MaintainAspectRatio
    {
        get => _maintainAspectRatio;
        set { _maintainAspectRatio = value; OnPropertyChanged(); }
    }
}