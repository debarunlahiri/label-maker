// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using LabelMaker.Models;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Rendering;
using RoundRectangle = Microsoft.Maui.Controls.Shapes.RoundRectangle;

namespace LabelMaker;

public partial class MainPage
{
    #region Element Creation

    private void OnAddTextClicked(object sender, EventArgs e)
    {
        var element = new TextElement { Text = "Sample Text" };
        AddElement(element);
    }

    private void OnAddRectangleClicked(object sender, EventArgs e)
    {
        var element = new ShapeElement(ElementType.Rectangle);
        AddElement(element);
    }

    private void OnAddCircleClicked(object sender, EventArgs e)
    {
        var element = new ShapeElement(ElementType.Circle);
        AddElement(element);
    }

    private void OnAddLineClicked(object sender, EventArgs e)
    {
        var element = new ShapeElement(ElementType.Line);
        AddElement(element);
    }

    private void OnAddImageClicked(object sender, EventArgs e)
    {
        var element = new ImageElement();
        AddElement(element);
    }

    private void OnAddBarcodeClicked(object sender, EventArgs e)
    {
        var element = new BarcodeElement { Data = "1234567890" };
        AddElement(element);
    }

    private void OnAddQRCodeClicked(object sender, EventArgs e)
    {
        var element = new QRCodeElement { Data = "https://example.com" };
        AddElement(element);
    }

    private void OnAddRoundedRectangleClicked(object sender, EventArgs e)
    {
        var element = new ShapeElement(ElementType.RoundedRectangle);
        AddElement(element);
    }

    private void OnAddEllipseClicked(object sender, EventArgs e)
    {
        var element = new ShapeElement(ElementType.Ellipse);
        AddElement(element);
    }

    private void OnAddTriangleClicked(object sender, EventArgs e)
    {
        var element = new ShapeElement(ElementType.Triangle);
        AddElement(element);
    }

    private void OnAddDateTimeClicked(object sender, EventArgs e)
    {
        var element = new DateTimeElement();
        AddElement(element);
    }

    private void OnAddCounterClicked(object sender, EventArgs e)
    {
        var element = new CounterElement();
        AddElement(element);
    }

    private void OnAddDatabaseFieldClicked(object sender, EventArgs e)
    {
        var element = new DatabaseFieldElement();
        AddElement(element);
    }

    private void OnAddRFIDClicked(object sender, EventArgs e)
    {
        var element = new RFIDElement();
        AddElement(element);
    }

    private void AddElement(LabelElement element)
    {
        RecordUndoState();
        element.X = 50;
        element.Y = 50;
        _currentTemplate.Elements.Add(element);
        CreateVisualElement(element);
        UpdateObjectList();
        SelectElement(element);
    }

    private void CreateVisualElement(LabelElement element)
    {
        View visualElement = CreateVisualForElement(element);

        var container = new AbsoluteLayout
        {
            WidthRequest = element.Width,
            HeightRequest = element.Height,
            BackgroundColor = Colors.Transparent
        };

        AbsoluteLayout.SetLayoutBounds(visualElement, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(visualElement, AbsoluteLayoutFlags.All);
        container.Children.Add(visualElement);

        var selectionBorder = new Border
        {
            Stroke = Colors.DodgerBlue,
            StrokeThickness = 1,
            BackgroundColor = Colors.Transparent,
            IsVisible = false
        };
        AbsoluteLayout.SetLayoutBounds(selectionBorder, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(selectionBorder, AbsoluteLayoutFlags.All);
        container.Children.Add(selectionBorder);

        var selectionHandles = new List<View>
        {
            CreateResizeHandle(element, container, ResizeHandleKind.TopLeft),
            CreateResizeHandle(element, container, ResizeHandleKind.TopCenter),
            CreateResizeHandle(element, container, ResizeHandleKind.TopRight),
            CreateResizeHandle(element, container, ResizeHandleKind.MiddleLeft),
            CreateResizeHandle(element, container, ResizeHandleKind.MiddleRight),
            CreateResizeHandle(element, container, ResizeHandleKind.BottomLeft),
            CreateResizeHandle(element, container, ResizeHandleKind.BottomCenter),
            CreateResizeHandle(element, container, ResizeHandleKind.BottomRight)
        };

        foreach (var handle in selectionHandles)
            container.Children.Add(handle);

        var rotateHandle = CreateRotateHandle(element);
        var rotateStem = new BoxView
        {
            WidthRequest = 2,
            HeightRequest = 20,
            BackgroundColor = Color.FromArgb("#2563EB"),
            IsVisible = false,
            InputTransparent = true,
            Margin = new Thickness(0, -20, 0, 0)
        };
        AbsoluteLayout.SetLayoutBounds(rotateStem, new Rect(0.5, 0, 2, 20));
        AbsoluteLayout.SetLayoutFlags(rotateStem, AbsoluteLayoutFlags.PositionProportional);
        container.Children.Add(rotateStem);
        container.Children.Add(rotateHandle);
        selectionHandles.Add(rotateStem);
        selectionHandles.Add(rotateHandle);

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => SelectElement(element);
        container.GestureRecognizers.Add(tapGesture);

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += (s, e) =>
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    if (_isManipulatingHandle)
                        return;

                    _isDragging = true;
                    _elementStart = new Point(element.X, element.Y);
                    SelectElement(element);
                    break;
                case GestureStatus.Running:
                    if (_isManipulatingHandle)
                        return;

                    element.X = Math.Max(0, _elementStart.X + e.TotalX);
                    element.Y = Math.Max(0, _elementStart.Y + e.TotalY);
                    UpdateElementPosition(element, container);
                    UpdateStatusBar();
                    break;
                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    _isDragging = false;
                    break;
            }
        };
        container.GestureRecognizers.Add(panGesture);

        AbsoluteLayout.SetLayoutBounds(container, new Rect(element.X, element.Y, element.Width, element.Height));
        AbsoluteLayout.SetLayoutFlags(container, AbsoluteLayoutFlags.None);
        container.Rotation = element.Rotation;
        ApplyElementFlip(element, visualElement);
        
        CanvasArea.Children.Add(container);
        _elementViews[element] = container;

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(LabelElement.X) || 
                e.PropertyName == nameof(LabelElement.Y) ||
                e.PropertyName == nameof(LabelElement.Width) ||
                e.PropertyName == nameof(LabelElement.Height))
            {
                UpdateElementPosition(element, container);
                UpdateStatusBar();
            }
            else if (e.PropertyName == nameof(LabelElement.IsSelected))
            {
                selectionBorder.IsVisible = element.IsSelected;
                foreach (var handle in selectionHandles)
                    handle.IsVisible = element.IsSelected;
            }
            else if (e.PropertyName == nameof(LabelElement.Rotation))
            {
                container.Rotation = element.Rotation;
            }
            else if (e.PropertyName == nameof(LabelElement.IsFlippedHorizontal) ||
                     e.PropertyName == nameof(LabelElement.IsFlippedVertical))
            {
                ApplyElementFlip(element, visualElement);
            }
        };
    }

    private View CreateHandle(double size = 10, bool isRound = false)
    {
        return new Border
        {
            WidthRequest = size,
            HeightRequest = size,
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#2563EB"),
            StrokeThickness = 1,
            StrokeShape = isRound ? new RoundRectangle { CornerRadius = new CornerRadius(size / 2) } : null,
            IsVisible = false,
            Margin = new Thickness(-size / 2)
        };
    }

    private View CreateResizeHandle(LabelElement element, View container, ResizeHandleKind kind)
    {
        var handle = CreateHandle();
        var (x, y) = GetHandleAnchor(kind);
        AbsoluteLayout.SetLayoutBounds(handle, new Rect(x, y, 10, 10));
        AbsoluteLayout.SetLayoutFlags(handle, AbsoluteLayoutFlags.PositionProportional);

        double startX = 0;
        double startY = 0;
        double startWidth = 0;
        double startHeight = 0;

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += (s, e) =>
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _isManipulatingHandle = true;
                    SelectElement(element);
                    startX = element.X;
                    startY = element.Y;
                    startWidth = element.Width;
                    startHeight = element.Height;
                    break;
                case GestureStatus.Running:
                    ResizeElement(element, kind, startX, startY, startWidth, startHeight, e.TotalX, e.TotalY);
                    UpdateElementPosition(element, container);
                    UpdatePropertiesPanel();
                    UpdateStatusBar();
                    break;
                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    _isManipulatingHandle = false;
                    break;
            }
        };
        handle.GestureRecognizers.Add(panGesture);

        return handle;
    }

    private View CreateRotateHandle(LabelElement element)
    {
        var handle = CreateHandle(14, true);
        handle.Margin = new Thickness(-7, -34, -7, 0);
        AbsoluteLayout.SetLayoutBounds(handle, new Rect(0.5, 0, 14, 14));
        AbsoluteLayout.SetLayoutFlags(handle, AbsoluteLayoutFlags.PositionProportional);

        double startRotation = 0;
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += (s, e) =>
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _isManipulatingHandle = true;
                    SelectElement(element);
                    startRotation = element.Rotation;
                    break;
                case GestureStatus.Running:
                    element.Rotation = NormalizeDegrees(startRotation + e.TotalX);
                    RotationSlider.Value = element.Rotation;
                    RotationEntry.Text = $"{element.Rotation:F0}";
                    UpdateStatusBar();
                    break;
                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    _isManipulatingHandle = false;
                    break;
            }
        };
        handle.GestureRecognizers.Add(panGesture);

        return handle;
    }

    private static (double X, double Y) GetHandleAnchor(ResizeHandleKind kind)
    {
        return kind switch
        {
            ResizeHandleKind.TopLeft => (0, 0),
            ResizeHandleKind.TopCenter => (0.5, 0),
            ResizeHandleKind.TopRight => (1, 0),
            ResizeHandleKind.MiddleLeft => (0, 0.5),
            ResizeHandleKind.MiddleRight => (1, 0.5),
            ResizeHandleKind.BottomLeft => (0, 1),
            ResizeHandleKind.BottomCenter => (0.5, 1),
            ResizeHandleKind.BottomRight => (1, 1),
            _ => (0.5, 0.5)
        };
    }

    private static void ResizeElement(
        LabelElement element,
        ResizeHandleKind kind,
        double startX,
        double startY,
        double startWidth,
        double startHeight,
        double deltaX,
        double deltaY)
    {
        const double minSize = 12;

        var x = startX;
        var y = startY;
        var width = startWidth;
        var height = startHeight;

        if (kind is ResizeHandleKind.TopLeft or ResizeHandleKind.MiddleLeft or ResizeHandleKind.BottomLeft)
        {
            width = Math.Max(minSize, startWidth - deltaX);
            x = startX + (startWidth - width);
        }
        else if (kind is ResizeHandleKind.TopRight or ResizeHandleKind.MiddleRight or ResizeHandleKind.BottomRight)
        {
            width = Math.Max(minSize, startWidth + deltaX);
        }

        if (kind is ResizeHandleKind.TopLeft or ResizeHandleKind.TopCenter or ResizeHandleKind.TopRight)
        {
            height = Math.Max(minSize, startHeight - deltaY);
            y = startY + (startHeight - height);
        }
        else if (kind is ResizeHandleKind.BottomLeft or ResizeHandleKind.BottomCenter or ResizeHandleKind.BottomRight)
        {
            height = Math.Max(minSize, startHeight + deltaY);
        }

        element.X = Math.Max(0, x);
        element.Y = Math.Max(0, y);
        element.Width = width;
        element.Height = height;
    }

    private static double NormalizeDegrees(double value)
    {
        var normalized = value % 360;
        return normalized < 0 ? normalized + 360 : normalized;
    }

    private static void ApplyElementFlip(LabelElement element, View visualElement)
    {
        visualElement.ScaleX = element.IsFlippedHorizontal ? -1 : 1;
        visualElement.ScaleY = element.IsFlippedVertical ? -1 : 1;
    }

    private View CreateVisualForElement(LabelElement element)
    {
        return element.Type switch
        {
            ElementType.Text => CreateTextVisual((TextElement)element),
            ElementType.Rectangle => CreateRectangleVisual((ShapeElement)element),
            ElementType.RoundedRectangle => CreateRoundedRectangleVisual((ShapeElement)element),
            ElementType.Circle => CreateCircleVisual((ShapeElement)element),
            ElementType.Ellipse => CreateEllipseVisual((ShapeElement)element),
            ElementType.Triangle => CreateTriangleVisual((ShapeElement)element),
            ElementType.Line => CreateLineVisual((ShapeElement)element),
            ElementType.Image => CreateImageVisual((ImageElement)element),
            ElementType.Barcode => CreateBarcodeVisual((BarcodeElement)element),
            ElementType.QRCode => CreateQRCodeVisual((QRCodeElement)element),
            ElementType.DateTime => CreateDateTimeVisual((DateTimeElement)element),
            ElementType.Counter => CreateCounterVisual((CounterElement)element),
            ElementType.DatabaseField => CreateDatabaseFieldVisual((DatabaseFieldElement)element),
            ElementType.RFID => CreateRFIDVisual((RFIDElement)element),
            _ => new BoxView()
        };
    }

    private View CreateTextVisual(TextElement element)
    {
        var label = new Label
        {
            Text = element.Text,
            FontSize = element.FontSize,
            TextColor = element.TextColor,
            FontAttributes = (element.Bold ? FontAttributes.Bold : FontAttributes.None) |
                           (element.Italic ? FontAttributes.Italic : FontAttributes.None),
            HorizontalTextAlignment = element.HorizontalAlignment,
            VerticalTextAlignment = element.VerticalAlignment,
            LineBreakMode = LineBreakMode.WordWrap,
            Padding = 2
        };

        element.PropertyChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (e.PropertyName == nameof(TextElement.Text)) label.Text = element.Text;
                if (e.PropertyName == nameof(TextElement.FontSize)) label.FontSize = element.FontSize;
                if (e.PropertyName == nameof(TextElement.TextColor)) label.TextColor = element.TextColor;
                if (e.PropertyName == nameof(TextElement.Bold) || e.PropertyName == nameof(TextElement.Italic))
                    label.FontAttributes = (element.Bold ? FontAttributes.Bold : FontAttributes.None) |
                                         (element.Italic ? FontAttributes.Italic : FontAttributes.None);
                if (e.PropertyName == nameof(TextElement.HorizontalAlignment)) label.HorizontalTextAlignment = element.HorizontalAlignment;
                if (e.PropertyName == nameof(TextElement.Underline)) label.TextDecorations = element.Underline ? TextDecorations.Underline : TextDecorations.None;
            });
        };

        return label;
    }

    private View CreateRectangleVisual(ShapeElement element)
    {
        var border = new Border
        {
            BackgroundColor = element.Filled ? element.BackgroundColor : Colors.Transparent,
            Stroke = element.BorderColor,
            StrokeThickness = element.BorderWidth
        };

        element.PropertyChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (e.PropertyName == nameof(ShapeElement.Filled)) border.BackgroundColor = element.Filled ? element.BackgroundColor : Colors.Transparent;
                if (e.PropertyName == nameof(LabelElement.BorderColor)) border.Stroke = element.BorderColor;
                if (e.PropertyName == nameof(LabelElement.BorderWidth)) border.StrokeThickness = element.BorderWidth;
                if (e.PropertyName == nameof(LabelElement.BackgroundColor)) border.BackgroundColor = element.Filled ? element.BackgroundColor : Colors.Transparent;
            });
        };

        return border;
    }

    private View CreateCircleVisual(ShapeElement element)
    {
        var border = new Border
        {
            BackgroundColor = element.Filled ? element.BackgroundColor : Colors.Transparent,
            Stroke = element.BorderColor,
            StrokeThickness = element.BorderWidth,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(50) }
        };

        element.PropertyChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (e.PropertyName == nameof(ShapeElement.Filled)) border.BackgroundColor = element.Filled ? element.BackgroundColor : Colors.Transparent;
                if (e.PropertyName == nameof(LabelElement.BorderColor)) border.Stroke = element.BorderColor;
                if (e.PropertyName == nameof(LabelElement.BorderWidth)) border.StrokeThickness = element.BorderWidth;
                if (e.PropertyName == nameof(LabelElement.BackgroundColor)) border.BackgroundColor = element.Filled ? element.BackgroundColor : Colors.Transparent;
            });
        };

        return border;
    }

    private View CreateLineVisual(ShapeElement element)
    {
        var boxView = new BoxView
        {
            BackgroundColor = element.BorderColor,
            HeightRequest = element.BorderWidth
        };

        element.PropertyChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (e.PropertyName == nameof(LabelElement.BorderColor)) boxView.BackgroundColor = element.BorderColor;
                if (e.PropertyName == nameof(LabelElement.BorderWidth)) boxView.HeightRequest = element.BorderWidth;
            });
        };

        return boxView;
    }

    private View CreateImageVisual(ImageElement element)
    {
        var grid = new Grid
        {
            BackgroundColor = Color.FromArgb("#E5E7EB")
        };

        var image = new Image
        {
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.Transparent
        };

        var placeholder = new Label
        {
            Text = "Choose image",
            FontSize = 12,
            TextColor = Color.FromArgb("#6B7280"),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        grid.Children.Add(image);
        grid.Children.Add(placeholder);

        UpdateImageSource(element, image, placeholder);

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ImageElement.ImagePath))
            {
                MainThread.BeginInvokeOnMainThread(() => UpdateImageSource(element, image, placeholder));
            }
        };

        return grid;
    }

    private static void UpdateImageSource(ImageElement element, Image image, Label placeholder)
    {
        if (string.IsNullOrWhiteSpace(element.ImagePath) || !File.Exists(element.ImagePath))
        {
            image.Source = null;
            placeholder.IsVisible = true;
            return;
        }

        image.Source = ImageSource.FromFile(element.ImagePath);
        placeholder.IsVisible = false;
    }

    private View CreateBarcodeVisual(BarcodeElement element)
    {
        var grid = new Grid();
        var barcodeImage = new Image
        {
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.White
        };

        UpdateBarcodeImage(element, barcodeImage);
        grid.Children.Add(barcodeImage);

        if (element.ShowText)
        {
            var textLabel = new Label
            {
                Text = element.Data,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                FontSize = 10,
                TextColor = element.BarcodeColor,
                BackgroundColor = Colors.White,
                Margin = new Thickness(0, 0, 0, 2)
            };
            grid.Children.Add(textLabel);

            element.PropertyChanged += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (e.PropertyName == nameof(BarcodeElement.ShowText)) textLabel.IsVisible = element.ShowText;
                    if (e.PropertyName == nameof(BarcodeElement.Data)) 
                    {
                        textLabel.Text = element.Data;
                        UpdateBarcodeImage(element, barcodeImage);
                    }
                    if (e.PropertyName == nameof(BarcodeElement.BarcodeColor)) textLabel.TextColor = element.BarcodeColor;
                });
            };
        }
        else
        {
            element.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BarcodeElement.Data))
                    UpdateBarcodeImage(element, barcodeImage);
            };
        }

        return grid;
    }

    private void UpdateBarcodeImage(BarcodeElement element, Image image)
    {
        try
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = (int)element.Height,
                    Width = (int)element.Width,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(element.Data);
            var pngBytes = PngImageEncoder.Encode(pixelData);
            image.Source = ImageSource.FromStream(() => new MemoryStream(pngBytes));
        }
        catch
        {
            image.Source = null;
        }
    }

    private View CreateQRCodeVisual(QRCodeElement element)
    {
        var image = new Image
        {
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.White
        };

        UpdateQRCodeImage(element, image);

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(QRCodeElement.Data))
                UpdateQRCodeImage(element, image);
        };

        return image;
    }

    private void UpdateQRCodeImage(QRCodeElement element, Image image)
    {
        try
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = (int)element.Height,
                    Width = (int)element.Width,
                    Margin = 1
                }
            };

            var pixelData = writer.Write(element.Data);
            var pngBytes = PngImageEncoder.Encode(pixelData);
            image.Source = ImageSource.FromStream(() => new MemoryStream(pngBytes));
        }
        catch
        {
            image.Source = null;
        }
    }

    private View CreateRoundedRectangleVisual(ShapeElement element)
    {
        var border = new Border
        {
            BackgroundColor = element.Filled ? element.FillColor : Colors.Transparent,
            StrokeShape = new RoundRectangle { CornerRadius = element.CornerRadius > 0 ? element.CornerRadius : 10 },
            StrokeColor = element.BorderColor,
            StrokeThickness = (double)element.BorderWidth,
            Padding = new Thickness(0)
        };

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ShapeElement.Filled) || e.PropertyName == nameof(ShapeElement.FillColor))
                border.BackgroundColor = element.Filled ? element.FillColor : Colors.Transparent;
            if (e.PropertyName == nameof(ShapeElement.CornerRadius))
                border.StrokeShape = new RoundRectangle { CornerRadius = element.CornerRadius > 0 ? element.CornerRadius : 10 };
        };

        return border;
    }

    private View CreateEllipseVisual(ShapeElement element)
    {
        var ellipse = new Microsoft.Maui.Controls.Shapes.Ellipse
        {
            Fill = element.Filled ? element.FillColor : Colors.Transparent,
            Stroke = element.BorderColor,
            StrokeThickness = element.BorderWidth
        };

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ShapeElement.Filled) || e.PropertyName == nameof(ShapeElement.FillColor))
                ellipse.Fill = element.Filled ? element.FillColor : Colors.Transparent;
            if (e.PropertyName == nameof(ShapeElement.BorderColor))
                ellipse.Stroke = element.BorderColor;
            if (e.PropertyName == nameof(ShapeElement.BorderWidth))
                ellipse.StrokeThickness = element.BorderWidth;
        };

        return ellipse;
    }

    private View CreateTriangleVisual(ShapeElement element)
    {
        var polygon = new Microsoft.Maui.Controls.Shapes.Polygon
        {
            Fill = element.Filled ? element.FillColor : Colors.Transparent,
            Stroke = element.BorderColor,
            StrokeThickness = element.BorderWidth,
            Points = new Microsoft.Maui.Controls.Shapes.PointCollection
            {
                new Point(element.Width / 2, 0),
                new Point(element.Width, element.Height),
                new Point(0, element.Height)
            }
        };

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ShapeElement.Filled) || e.PropertyName == nameof(ShapeElement.FillColor))
                polygon.Fill = element.Filled ? element.FillColor : Colors.Transparent;
            if (e.PropertyName == nameof(ShapeElement.BorderColor))
                polygon.Stroke = element.BorderColor;
            if (e.PropertyName == nameof(ShapeElement.BorderWidth))
                polygon.StrokeThickness = element.BorderWidth;
            if (e.PropertyName == nameof(LabelElement.Width) || e.PropertyName == nameof(LabelElement.Height))
            {
                polygon.Points = new Microsoft.Maui.Controls.Shapes.PointCollection
                {
                    new Point(element.Width / 2, 0),
                    new Point(element.Width, element.Height),
                    new Point(0, element.Height)
                };
            }
        };

        return polygon;
    }

    private View CreateDateTimeVisual(DateTimeElement element)
    {
        var formatDisplay = element.ValueType switch
        {
            DateTimeValueType.CurrentDate => DateTime.Now.ToString(element.Format),
            DateTimeValueType.CurrentTime => DateTime.Now.ToString("HH:mm:ss"),
            DateTimeValueType.CurrentDateTime => DateTime.Now.ToString(element.Format + " HH:mm:ss"),
            DateTimeValueType.OffsetDate => DateTime.Now.AddDays(element.OffsetDays).ToString(element.Format),
            _ => DateTime.Now.ToString(element.Format)
        };

        var label = new Label
        {
            Text = formatDisplay,
            FontSize = element.FontSize,
            TextColor = element.TextColor,
            FontAttributes = element.Bold ? FontAttributes.Bold : FontAttributes.None,
            HorizontalTextAlignment = element.HorizontalAlignment,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DateTimeElement.Format) || e.PropertyName == nameof(DateTimeElement.ValueType) || e.PropertyName == nameof(DateTimeElement.OffsetDays))
            {
                var display = element.ValueType switch
                {
                    DateTimeValueType.CurrentDate => DateTime.Now.ToString(element.Format),
                    DateTimeValueType.CurrentTime => DateTime.Now.ToString("HH:mm:ss"),
                    DateTimeValueType.CurrentDateTime => DateTime.Now.ToString(element.Format + " HH:mm:ss"),
                    DateTimeValueType.OffsetDate => DateTime.Now.AddDays(element.OffsetDays).ToString(element.Format),
                    _ => DateTime.Now.ToString(element.Format)
                };
                label.Text = display;
            }
            if (e.PropertyName == nameof(DateTimeElement.FontSize)) label.FontSize = element.FontSize;
            if (e.PropertyName == nameof(DateTimeElement.TextColor)) label.TextColor = element.TextColor;
            if (e.PropertyName == nameof(DateTimeElement.Bold)) label.FontAttributes = element.Bold ? FontAttributes.Bold : FontAttributes.None;
            if (e.PropertyName == nameof(DateTimeElement.HorizontalAlignment)) label.HorizontalTextAlignment = element.HorizontalAlignment;
        };

        return label;
    }

    private View CreateCounterVisual(CounterElement element)
    {
        var display = $"{element.Prefix}{element.StartValue.ToString().PadLeft(element.Padding, '0')}{element.Suffix}";

        var label = new Label
        {
            Text = display,
            FontSize = element.FontSize,
            TextColor = element.TextColor,
            FontAttributes = element.Bold ? FontAttributes.Bold : FontAttributes.None,
            FontFamily = element.FontFamily,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(CounterElement.Prefix) || e.PropertyName == nameof(CounterElement.Suffix) ||
                e.PropertyName == nameof(CounterElement.Padding) || e.PropertyName == nameof(CounterElement.StartValue))
            {
                label.Text = $"{element.Prefix}{element.StartValue.ToString().PadLeft(element.Padding, '0')}{element.Suffix}";
            }
            if (e.PropertyName == nameof(CounterElement.FontSize)) label.FontSize = element.FontSize;
            if (e.PropertyName == nameof(CounterElement.TextColor)) label.TextColor = element.TextColor;
            if (e.PropertyName == nameof(CounterElement.Bold)) label.FontAttributes = element.Bold ? FontAttributes.Bold : FontAttributes.None;
        };

        return label;
    }

    private View CreateDatabaseFieldVisual(DatabaseFieldElement element)
    {
        var label = new Label
        {
            Text = $"{{{element.FieldName}}}",
            FontSize = element.FontSize,
            TextColor = element.TextColor,
            FontAttributes = element.Bold ? FontAttributes.Bold : FontAttributes.None,
            FontFamily = element.FontFamily,
            HorizontalTextAlignment = element.HorizontalAlignment,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Color.FromArgb("#E8F0FE"),
            Padding = new Thickness(4, 2)
        };

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DatabaseFieldElement.FieldName))
                label.Text = $"{{{element.FieldName}}}";
            if (e.PropertyName == nameof(DatabaseFieldElement.FontSize)) label.FontSize = element.FontSize;
            if (e.PropertyName == nameof(DatabaseFieldElement.TextColor)) label.TextColor = element.TextColor;
            if (e.PropertyName == nameof(DatabaseFieldElement.Bold)) label.FontAttributes = element.Bold ? FontAttributes.Bold : FontAttributes.None;
            if (e.PropertyName == nameof(DatabaseFieldElement.HorizontalAlignment)) label.HorizontalTextAlignment = element.HorizontalAlignment;
        };

        return label;
    }

    private View CreateRFIDVisual(RFIDElement element)
    {
        var label = new Label
        {
            Text = $"RFID: {element.EpcValue}",
            FontSize = element.FontSize,
            TextColor = element.TextColor,
            FontAttributes = element.Bold ? FontAttributes.Bold : FontAttributes.None,
            FontFamily = element.FontFamily,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Color.FromArgb("#FFF3E0"),
            Padding = new Thickness(4, 2)
        };

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(RFIDElement.EpcValue))
                label.Text = $"RFID: {element.EpcValue}";
            if (e.PropertyName == nameof(RFIDElement.FontSize)) label.FontSize = element.FontSize;
            if (e.PropertyName == nameof(RFIDElement.TextColor)) label.TextColor = element.TextColor;
            if (e.PropertyName == nameof(RFIDElement.Bold)) label.FontAttributes = element.Bold ? FontAttributes.Bold : FontAttributes.None;
        };

        return label;
    }

    private void UpdateElementPosition(LabelElement element, View container)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            container.WidthRequest = element.Width;
            container.HeightRequest = element.Height;
            AbsoluteLayout.SetLayoutBounds(container, new Rect(element.X, element.Y, element.Width, element.Height));
        });
    }

    #endregion
}