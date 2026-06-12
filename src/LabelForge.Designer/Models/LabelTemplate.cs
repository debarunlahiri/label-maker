// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public enum UnitType
{
    Pixels,
    Inches,
    Millimeters,
    Centimeters,
    Points
}

public enum TemplateStatus
{
    Draft,
    PendingApproval,
    Approved,
    Rejected,
    Archived,
    Locked
}

public class LabelTemplate : INotifyPropertyChanged
{
    private string _name = "New Label";
    private string _description = "";
    private double _width = 400;
    private double _height = 300;
    private Color _backgroundColor = Colors.White;
    private double _dpi = 96;
    private double _marginTop;
    private double _marginBottom;
    private double _marginLeft;
    private double _marginRight;
    private double _gridSpacing = 10;
    private bool _snapToGrid = true;
    private UnitType _unit = UnitType.Pixels;
    private int _copies = 1;
    private string _printerName = "";
    private double _labelGapX;
    private double _labelGapY;
    private int _rows = 1;
    private int _columns = 1;
    private TemplateStatus _status = TemplateStatus.Draft;
    private int _version = 1;
    private string _orientation = "Portrait";

    public ObservableCollection<LabelElement> Elements { get; } = new();

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
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

    public double MarginTop
    {
        get => _marginTop;
        set { _marginTop = value; OnPropertyChanged(); }
    }

    public double MarginBottom
    {
        get => _marginBottom;
        set { _marginBottom = value; OnPropertyChanged(); }
    }

    public double MarginLeft
    {
        get => _marginLeft;
        set { _marginLeft = value; OnPropertyChanged(); }
    }

    public double MarginRight
    {
        get => _marginRight;
        set { _marginRight = value; OnPropertyChanged(); }
    }

    public double GridSpacing
    {
        get => _gridSpacing;
        set { _gridSpacing = value; OnPropertyChanged(); }
    }

    public bool SnapToGrid
    {
        get => _snapToGrid;
        set { _snapToGrid = value; OnPropertyChanged(); }
    }

    public UnitType Unit
    {
        get => _unit;
        set { _unit = value; OnPropertyChanged(); }
    }

    public int Copies
    {
        get => _copies;
        set { _copies = value; OnPropertyChanged(); }
    }

    public string PrinterName
    {
        get => _printerName;
        set { _printerName = value; OnPropertyChanged(); }
    }

    public double LabelGapX
    {
        get => _labelGapX;
        set { _labelGapX = value; OnPropertyChanged(); }
    }

    public double LabelGapY
    {
        get => _labelGapY;
        set { _labelGapY = value; OnPropertyChanged(); }
    }

    public int Rows
    {
        get => _rows;
        set { _rows = value; OnPropertyChanged(); }
    }

    public int Columns
    {
        get => _columns;
        set { _columns = value; OnPropertyChanged(); }
    }

    public TemplateStatus Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public int Version
    {
        get => _version;
        set { _version = value; OnPropertyChanged(); }
    }

    public string Orientation
    {
        get => _orientation;
        set { _orientation = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}