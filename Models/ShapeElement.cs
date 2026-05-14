// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class ShapeElement : LabelElement
{
    private bool _filled;

    public ShapeElement(ElementType type)
    {
        Type = type;
        switch (type)
        {
            case ElementType.Rectangle:
                Width = 100;
                Height = 50;
                break;
            case ElementType.Circle:
                Width = 80;
                Height = 80;
                break;
            case ElementType.Line:
                Width = 100;
                Height = 2;
                break;
        }
    }

    public bool Filled
    {
        get => _filled;
        set { _filled = value; OnPropertyChanged(); }
    }
}
