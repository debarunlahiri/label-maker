// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class ImageElement : LabelElement
{
    private string _imagePath = "";

    public ImageElement()
    {
        Type = ElementType.Image;
        Width = 100;
        Height = 100;
    }

    public string ImagePath
    {
        get => _imagePath;
        set { _imagePath = value; OnPropertyChanged(); }
    }
}
