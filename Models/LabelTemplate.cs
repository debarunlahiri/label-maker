// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class LabelTemplate : INotifyPropertyChanged
{
    private string _name = "New Label";
    private double _width = 400;
    private double _height = 300;
    private Color _backgroundColor = Colors.White;
    private double _dpi = 96;

    public ObservableCollection<LabelElement> Elements { get; } = new();

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public double Width
    {
        get => _width;
        set { _width = value; OnPropertyChanged(); }
    }

    public double Height
    {
        get => _height;
        set { _height = value; OnPropertyChanged(); }
    }

    public Color BackgroundColor
    {
        get => _backgroundColor;
        set { _backgroundColor = value; OnPropertyChanged(); }
    }

    public double Dpi
    {
        get => _dpi;
        set { _dpi = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
