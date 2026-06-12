// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Layouts;
using LabelMaker.Models;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Rendering;
using RoundRectangle = Microsoft.Maui.Controls.Shapes.RoundRectangle;

namespace LabelMaker;

public partial class MainPage : ContentPage
{
    private LabelTemplate _currentTemplate;
    private LabelElement? _selectedElement;
    private Dictionary<LabelElement, View> _elementViews = new();
    private double _zoomLevel = 1.0;
    private bool _isDragging;
    private bool _isManipulatingHandle;
    private Point _elementStart;

    // Undo/Redo system
    private Stack<string> _undoStack = new();
    private Stack<string> _redoStack = new();

    // Clipboard
    private static LabelElement? _clipboardElement;

    // View state tracking
    private bool _rulersVisible = true;
    private bool _objectTreeVisible = true;
    private bool _propertiesVisible = true;
    private bool _isApplyingUndo;

    // File state tracking
    private string? _currentFilePath;
    private bool _isModified;

    public ObservableCollection<ObjectListItem> ObjectListItems { get; } = new();

    public MainPage()
    {
        InitializeComponent();
        _currentTemplate = new LabelTemplate();
        SetupCanvas();
        SetupRulers();
        SetupGrid();
        ObjectList.ItemsSource = ObjectListItems;
    }

    private void SetupCanvas()
    {
        CanvasBorder.WidthRequest = _currentTemplate.Width;
        CanvasBorder.HeightRequest = _currentTemplate.Height;
        CanvasArea.WidthRequest = _currentTemplate.Width;
        CanvasArea.HeightRequest = _currentTemplate.Height;
        SetupRulers();
        SetupGrid();
    }

    private void SetupRulers()
    {
        // Rulers will be drawn using GraphicsView
        TopRuler.Drawable = new HorizontalRulerDrawable((float)_currentTemplate.Width);
        BottomRuler.Drawable = new HorizontalRulerDrawable((float)_currentTemplate.Width);
        LeftRuler.Drawable = new VerticalRulerDrawable((float)_currentTemplate.Height);
        RightRuler.Drawable = new VerticalRulerDrawable((float)_currentTemplate.Height);
    }

    private void SetupGrid()
    {
        GridBackground.Drawable = new GridDrawable((float)_currentTemplate.Width, (float)_currentTemplate.Height);
    }

    #region Object List

    private void UpdateObjectList()
    {
        ObjectListItems.Clear();
        foreach (var element in _currentTemplate.Elements)
        {
            var display = !string.IsNullOrWhiteSpace(element.Name)
                ? element.Name
                : element switch
                {
                    TextElement t => $"Text: {t.Text}",
                    BarcodeElement b => $"Barcode: {b.Data}",
                    QRCodeElement q => $"QR: {q.Data}",
                    ImageElement i => $"Image: {Path.GetFileName(i.ImagePath)}",
                    ShapeElement s => s.Type.ToString(),
                    DateTimeElement dt => $"Date: {dt.Format}",
                    CounterElement c => $"Counter: {c.Prefix}{c.StartValue}{c.Suffix}",
                    DatabaseFieldElement db => $"DB: {db.FieldName}",
                    RFIDElement rfid => $"RFID: {rfid.EpcValue}",
                    _ => element.Type.ToString()
                };
            ObjectListItems.Add(new ObjectListItem { Id = element.Id, Name = display });
        }
    }

    private void OnObjectListSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ObjectListItem item)
        {
            var element = _currentTemplate.Elements.FirstOrDefault(el => el.Id == item.Id);
            if (element != null)
            {
                SelectElement(element);
            }
        }
    }

    #endregion
}

public class ObjectListItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
}

public enum ResizeHandleKind
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

public class HorizontalRulerDrawable : IDrawable
{
    private readonly float _width;

    public HorizontalRulerDrawable(float width)
    {
        _width = width;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Color.FromArgb("#E5E7EB");
        canvas.FillRectangle(dirtyRect);
        
        canvas.StrokeColor = Color.FromArgb("#9CA3AF");
        canvas.StrokeSize = 1;
        canvas.FontColor = Color.FromArgb("#4B5563");
        
        for (int i = 0; i < _width; i += 10)
        {
            float y = i % 50 == 0 ? 0 : (i % 25 == 0 ? 10 : 20);
            canvas.DrawLine(i, y, i, 30);
            
            if (i % 50 == 0)
            {
                canvas.FontSize = 8;
                canvas.DrawString(i.ToString(), i + 2, 12, HorizontalAlignment.Left);
            }
        }
    }
}

public class VerticalRulerDrawable : IDrawable
{
    private readonly float _height;

    public VerticalRulerDrawable(float height)
    {
        _height = height;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Color.FromArgb("#E5E7EB");
        canvas.FillRectangle(dirtyRect);
        
        canvas.StrokeColor = Color.FromArgb("#9CA3AF");
        canvas.StrokeSize = 1;
        canvas.FontColor = Color.FromArgb("#4B5563");
        
        for (int i = 0; i < _height; i += 10)
        {
            float x = i % 50 == 0 ? 0 : (i % 25 == 0 ? 10 : 20);
            canvas.DrawLine(x, i, 30, i);
            
            if (i % 50 == 0)
            {
                canvas.FontSize = 8;
                canvas.DrawString(i.ToString(), 5, i + 2, HorizontalAlignment.Left);
            }
        }
    }
}

public class GridDrawable : IDrawable
{
    private readonly float _width;
    private readonly float _height;

    public GridDrawable(float width, float height)
    {
        _width = width;
        _height = height;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Color.FromArgb("#E5E7EB");
        canvas.StrokeSize = 0.5f;
        
        for (int x = 0; x < _width; x += 20)
        {
            canvas.DrawLine(x, 0, x, _height);
        }
        
        for (int y = 0; y < _height; y += 20)
        {
            canvas.DrawLine(0, y, _width, y);
        }
    }
}