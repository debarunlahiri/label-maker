using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using LabelMaker.Models;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Rendering;

namespace LabelMaker;

public partial class MainPage : ContentPage
{
    private LabelTemplate _currentTemplate;
    private LabelElement? _selectedElement;
    private Dictionary<LabelElement, View> _elementViews = new();
    private double _zoomLevel = 1.0;
    private bool _isDragging;
    private Point _elementStart;

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

    private void AddElement(LabelElement element)
    {
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

        // Selection border with resize handles
        var container = new AbsoluteLayout
        {
            WidthRequest = element.Width,
            HeightRequest = element.Height,
            BackgroundColor = Colors.Transparent
        };

        // Main visual element fills the container
        AbsoluteLayout.SetLayoutBounds(visualElement, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(visualElement, AbsoluteLayoutFlags.All);
        container.Children.Add(visualElement);

        // Selection border
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

        // Corner handles
        var topLeftHandle = CreateHandle();
        var topRightHandle = CreateHandle();
        var bottomLeftHandle = CreateHandle();
        var bottomRightHandle = CreateHandle();

        // Position handles at corners
        AbsoluteLayout.SetLayoutBounds(topLeftHandle, new Rect(0, 0, 8, 8));
        AbsoluteLayout.SetLayoutFlags(topLeftHandle, AbsoluteLayoutFlags.PositionProportional);
        container.Children.Add(topLeftHandle);

        AbsoluteLayout.SetLayoutBounds(topRightHandle, new Rect(1, 0, 8, 8));
        AbsoluteLayout.SetLayoutFlags(topRightHandle, AbsoluteLayoutFlags.PositionProportional);
        container.Children.Add(topRightHandle);

        AbsoluteLayout.SetLayoutBounds(bottomLeftHandle, new Rect(0, 1, 8, 8));
        AbsoluteLayout.SetLayoutFlags(bottomLeftHandle, AbsoluteLayoutFlags.PositionProportional);
        container.Children.Add(bottomLeftHandle);

        AbsoluteLayout.SetLayoutBounds(bottomRightHandle, new Rect(1, 1, 8, 8));
        AbsoluteLayout.SetLayoutFlags(bottomRightHandle, AbsoluteLayoutFlags.PositionProportional);
        container.Children.Add(bottomRightHandle);

        // Add gesture recognizers
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => SelectElement(element);
        container.GestureRecognizers.Add(tapGesture);

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += (s, e) =>
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _isDragging = true;
                    _elementStart = new Point(element.X, element.Y);
                    SelectElement(element);
                    break;
                case GestureStatus.Running:
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
        
        CanvasArea.Children.Add(container);
        _elementViews[element] = container;

        // Listen for property changes
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
                topLeftHandle.IsVisible = element.IsSelected;
                topRightHandle.IsVisible = element.IsSelected;
                bottomLeftHandle.IsVisible = element.IsSelected;
                bottomRightHandle.IsVisible = element.IsSelected;
            }
            else if (e.PropertyName == nameof(LabelElement.Rotation))
            {
                container.Rotation = element.Rotation;
            }
        };
    }

    private View CreateHandle()
    {
        return new BoxView
        {
            WidthRequest = 8,
            HeightRequest = 8,
            BackgroundColor = Colors.White,
            IsVisible = false,
            Margin = new Thickness(-4)
        };
    }

    private View CreateVisualForElement(LabelElement element)
    {
        return element.Type switch
        {
            ElementType.Text => CreateTextVisual((TextElement)element),
            ElementType.Rectangle => CreateRectangleVisual((ShapeElement)element),
            ElementType.Circle => CreateCircleVisual((ShapeElement)element),
            ElementType.Line => CreateLineVisual((ShapeElement)element),
            ElementType.Image => CreateImageVisual((ImageElement)element),
            ElementType.Barcode => CreateBarcodeVisual((BarcodeElement)element),
            ElementType.QRCode => CreateQRCodeVisual((QRCodeElement)element),
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
        var image = new Image
        {
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.LightGray
        };

        if (!string.IsNullOrEmpty(element.ImagePath))
        {
            try
            {
                image.Source = ImageSource.FromFile(element.ImagePath);
            }
            catch { }
        }

        element.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ImageElement.ImagePath) && !string.IsNullOrEmpty(element.ImagePath))
            {
                try
                {
                    image.Source = ImageSource.FromFile(element.ImagePath);
                }
                catch { }
            }
        };

        return image;
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
            var bitmap = new SkiaSharp.SKBitmap(pixelData.Width, pixelData.Height);
            var ptr = bitmap.GetPixels();
            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, ptr, pixelData.Pixels.Length);
            
            using var data = bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream(data.ToArray());
            image.Source = ImageSource.FromStream(() => new MemoryStream(data.ToArray()));
        }
        catch
        {
            // Fallback to simple representation
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
            var bitmap = new SkiaSharp.SKBitmap(pixelData.Width, pixelData.Height);
            var ptr = bitmap.GetPixels();
            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, ptr, pixelData.Pixels.Length);
            
            using var data = bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
            image.Source = ImageSource.FromStream(() => new MemoryStream(data.ToArray()));
        }
        catch
        {
            image.Source = null;
        }
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

    #region Object List

    private void UpdateObjectList()
    {
        ObjectListItems.Clear();
        foreach (var element in _currentTemplate.Elements)
        {
            var name = element switch
            {
                TextElement t => $"Text: {t.Text}",
                BarcodeElement b => $"Barcode: {b.Data}",
                QRCodeElement q => $"QR: {q.Data}",
                ImageElement i => $"Image: {Path.GetFileName(i.ImagePath)}",
                ShapeElement s => s.Type.ToString(),
                _ => element.Type.ToString()
            };
            ObjectListItems.Add(new ObjectListItem { Id = element.Id, Name = name });
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

    #region Selection and Properties

    private void SelectElement(LabelElement? element)
    {
        if (_selectedElement != null)
        {
            _selectedElement.IsSelected = false;
        }

        _selectedElement = element;

        if (_selectedElement != null)
        {
            _selectedElement.IsSelected = true;
            UpdatePropertiesPanel();
            UpdateStatusBar();
        }
        else
        {
            HidePropertiesPanel();
            ResetStatusBar();
        }
    }

    private void OnCanvasTapped(object sender, TappedEventArgs e)
    {
        if (!_isDragging)
        {
            SelectElement(null);
        }
    }

    private void UpdatePropertiesPanel()
    {
        NoSelectionLabel.IsVisible = false;
        CommonProperties.IsVisible = true;

        // Update common properties
        PosXEntry.Text = _selectedElement.X.ToString("F2");
        PosYEntry.Text = _selectedElement.Y.ToString("F2");
        WidthEntry.Text = _selectedElement.Width.ToString("F2");
        HeightEntry.Text = _selectedElement.Height.ToString("F2");
        RotationSlider.Value = _selectedElement.Rotation;
        BorderWidthStepper.Value = _selectedElement.BorderWidth;

        // Show specific properties based on element type
        TextProperties.IsVisible = _selectedElement is TextElement;
        BarcodeProperties.IsVisible = _selectedElement is BarcodeElement;

        if (_selectedElement is TextElement textElement)
        {
            TextContentEntry.Text = textElement.Text;
            FontSizeSlider.Value = textElement.FontSize;
            BoldCheckBox.IsChecked = textElement.Bold;
            ItalicCheckBox.IsChecked = textElement.Italic;
            UnderlineCheckBox.IsChecked = textElement.Underline;
        }

        if (_selectedElement is BarcodeElement barcodeElement)
        {
            BarcodeDataEntry.Text = barcodeElement.Data;
            ShowTextCheckBox.IsChecked = barcodeElement.ShowText;
        }
    }

    private void HidePropertiesPanel()
    {
        NoSelectionLabel.IsVisible = true;
        CommonProperties.IsVisible = false;
        TextProperties.IsVisible = false;
        BarcodeProperties.IsVisible = false;
    }

    #endregion

    #region Property Change Handlers

    private void OnPositionChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement == null) return;

        if (double.TryParse(PosXEntry.Text, out double x))
            _selectedElement.X = Math.Max(0, x);
        if (double.TryParse(PosYEntry.Text, out double y))
            _selectedElement.Y = Math.Max(0, y);
        UpdateStatusBar();
    }

    private void OnSizeChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement == null) return;

        if (double.TryParse(WidthEntry.Text, out double w))
            _selectedElement.Width = Math.Max(1, w);
        if (double.TryParse(HeightEntry.Text, out double h))
            _selectedElement.Height = Math.Max(1, h);
        UpdateStatusBar();
    }

    private void OnRotationChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.Rotation = e.NewValue;
        RotationValue.Text = $"{e.NewValue:F1}°";
        UpdateStatusBar();
    }

    private void OnBorderWidthChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.BorderWidth = e.NewValue;
        BorderWidthValue.Text = $"{e.NewValue:F0} px";
    }

    private void OnColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null || sender is not Button button) return;
        _selectedElement.BackgroundColor = button.BackgroundColor;
    }

    private void OnBorderColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null || sender is not Button button) return;
        _selectedElement.BorderColor = button.BackgroundColor;
    }

    private void OnTextContentChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
        {
            textElement.Text = TextContentEntry.Text;
            UpdateObjectList();
        }
    }

    private void OnFontChanged(object sender, EventArgs e)
    {
        if (_selectedElement is TextElement textElement && FontPicker.SelectedItem is string font)
            textElement.FontFamily = font;
    }

    private void OnFontSizeChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
        {
            textElement.FontSize = e.NewValue;
            FontSizeValue.Text = $"{e.NewValue:F0} pt";
        }
    }

    private void OnBoldChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
            textElement.Bold = e.Value;
    }

    private void OnItalicChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
            textElement.Italic = e.Value;
    }

    private void OnUnderlineChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
            textElement.Underline = e.Value;
    }

    private void OnTextColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement is TextElement textElement && sender is Button button)
            textElement.TextColor = button.BackgroundColor;
    }

    private void OnAlignmentChanged(object sender, EventArgs e)
    {
        if (_selectedElement is TextElement textElement && AlignmentPicker.SelectedIndex >= 0)
        {
            textElement.HorizontalAlignment = AlignmentPicker.SelectedIndex switch
            {
                0 => TextAlignment.Start,
                1 => TextAlignment.Center,
                2 => TextAlignment.End,
                _ => TextAlignment.Start
            };
        }
    }

    private void OnBarcodeDataChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement)
        {
            barcodeElement.Data = BarcodeDataEntry.Text;
            UpdateObjectList();
        }
    }

    private void OnBarcodeTypeChanged(object sender, EventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement && BarcodeTypePicker.SelectedItem is string type)
            barcodeElement.BarcodeType = type;
    }

    private void OnShowTextChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement)
            barcodeElement.ShowText = e.Value;
    }

    private void OnBarcodeColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement && sender is Button button)
            barcodeElement.BarcodeColor = button.BackgroundColor;
    }

    #endregion

    #region Zoom and Grid

    private void OnZoomInClicked(object sender, EventArgs e)
    {
        _zoomLevel = Math.Min(_zoomLevel + 0.1, 3.0);
        ApplyZoom();
    }

    private void OnZoomOutClicked(object sender, EventArgs e)
    {
        _zoomLevel = Math.Max(_zoomLevel - 0.1, 0.3);
        ApplyZoom();
    }

    private void ApplyZoom()
    {
        ZoomLabel.Text = $"{_zoomLevel * 100:F0}%";
        StatusZoom.Text = $"Zoom: {_zoomLevel * 100:F0}%";
        
        CanvasBorder.Scale = _zoomLevel;
        SetupRulers();
        SetupGrid();
    }

    private void OnGridToggleChanged(object sender, CheckedChangedEventArgs e)
    {
        GridBackground.IsVisible = e.Value;
    }

    #endregion

    #region Status Bar

    private void UpdateStatusBar()
    {
        if (_selectedElement != null)
        {
            StatusPosition.Text = $"X: {_selectedElement.X:F2}, Y: {_selectedElement.Y:F2}";
            StatusDimensions.Text = $"Width: {_selectedElement.Width:F2}, Height: {_selectedElement.Height:F2}";
            StatusAngle.Text = $"Angle: {_selectedElement.Rotation:F1}°";
        }
    }

    private void ResetStatusBar()
    {
        StatusPosition.Text = "X: 0.00, Y: 0.00";
        StatusDimensions.Text = "Width: 0.00, Height: 0.00";
        StatusAngle.Text = "Angle: 0.0°";
    }

    #endregion

    #region Actions

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_selectedElement != null)
        {
            if (_elementViews.TryGetValue(_selectedElement, out View? view))
            {
                CanvasArea.Children.Remove(view);
                _elementViews.Remove(_selectedElement);
            }
            _currentTemplate.Elements.Remove(_selectedElement);
            SelectElement(null);
            UpdateObjectList();
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            var json = JsonSerializer.Serialize(_currentTemplate, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            string fileName = $"{_currentTemplate.Name.Replace(" ", "_")}.json";
            string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
            await File.WriteAllTextAsync(filePath, json);
            await DisplayAlert("Success", $"Label saved to: {filePath}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
        }
    }

    private async void OnLoadClicked(object sender, EventArgs e)
    {
        try
        {
            var files = Directory.GetFiles(FileSystem.Current.AppDataDirectory, "*.json");
            if (files.Length == 0)
            {
                await DisplayAlert("Info", "No saved labels found.", "OK");
                return;
            }

            var fileNames = files.Select(f => Path.GetFileName(f)).ToArray();
            var selected = await DisplayActionSheet("Select Label", "Cancel", null, fileNames);
            
            if (selected != null && selected != "Cancel")
            {
                var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, selected);
                var json = await File.ReadAllTextAsync(filePath);
                var template = JsonSerializer.Deserialize<LabelTemplate>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (template != null)
                {
                    LoadTemplate(template);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load: {ex.Message}", "OK");
        }
    }

    private void LoadTemplate(LabelTemplate template)
    {
        CanvasArea.Children.Clear();
        _elementViews.Clear();
        _currentTemplate = template;
        SetupCanvas();

        foreach (var element in template.Elements)
        {
            CreateVisualElement(element);
        }
        UpdateObjectList();
        SelectElement(null);
    }

    private async void OnPrintClicked(object sender, EventArgs e)
    {
        try
        {
            var html = GeneratePrintHtml();
            var filePath = Path.Combine(FileSystem.Current.CacheDirectory, "print.html");
            await File.WriteAllTextAsync(filePath, html);
            await Launcher.OpenAsync(new OpenFileRequest("Print Label", new ReadOnlyFile(filePath)));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to print: {ex.Message}", "OK");
        }
    }

    private string GeneratePrintHtml()
    {
        var html = $@"<!DOCTYPE html>
<html>
<head>
    <title>{_currentTemplate.Name}</title>
    <style>
        body {{ margin: 0; padding: 20px; font-family: Arial, sans-serif; }}
        .label {{
            position: relative;
            width: {_currentTemplate.Width}px;
            height: {_currentTemplate.Height}px;
            background-color: {_currentTemplate.BackgroundColor.ToRgbHex()};
            border: 1px solid #ccc;
            page-break-after: always;
        }}
        .element {{
            position: absolute;
            box-sizing: border-box;
            overflow: hidden;
        }}
        @media print {{
            body {{ padding: 0; }}
            .label {{ border: none; }}
        }}
    </style>
</head>
<body>
    <div class=""label"">";

        foreach (var element in _currentTemplate.Elements)
        {
            var styles = $"left:{element.X}px;top:{element.Y}px;width:{element.Width}px;height:{element.Height}px;";
            
            switch (element.Type)
            {
                case ElementType.Text:
                    var textEl = (TextElement)element;
                    var fontStyle = (textEl.Bold ? "font-weight:bold;" : "") + 
                                  (textEl.Italic ? "font-style:italic;" : "") +
                                  (textEl.Underline ? "text-decoration:underline;" : "");
                    html += $@"
        <div class=""element"" style=""{styles}{fontStyle}font-size:{textEl.FontSize}px;color:{textEl.TextColor.ToRgbHex()};font-family:{textEl.FontFamily},Arial,sans-serif;text-align:{textEl.HorizontalAlignment.ToString().ToLower()};display:flex;align-items:{(textEl.VerticalAlignment == TextAlignment.Start ? "flex-start" : textEl.VerticalAlignment == TextAlignment.End ? "flex-end" : "center")};padding:2px;"">
            {System.Net.WebUtility.HtmlEncode(textEl.Text)}
        </div>";
                    break;
                    
                case ElementType.Rectangle:
                    var rectEl = (ShapeElement)element;
                    html += $@"
        <div class=""element"" style=""{styles}background-color:{(rectEl.Filled ? rectEl.BackgroundColor.ToRgbHex() : "transparent")};border:{rectEl.BorderWidth}px solid {rectEl.BorderColor.ToRgbHex()};"">
        </div>";
                    break;
                    
                case ElementType.Circle:
                    var circleEl = (ShapeElement)element;
                    html += $@"
        <div class=""element"" style=""{styles}background-color:{(circleEl.Filled ? circleEl.BackgroundColor.ToRgbHex() : "transparent")};border:{circleEl.BorderWidth}px solid {circleEl.BorderColor.ToRgbHex()};border-radius:50%;"">
        </div>";
                    break;
                    
                case ElementType.Line:
                    html += $@"
        <div class=""element"" style=""{styles}background-color:{element.BorderColor.ToRgbHex()};height:{element.BorderWidth}px;"">
        </div>";
                    break;
                    
                case ElementType.Barcode:
                    var barEl = (BarcodeElement)element;
                    html += $@"
        <div class=""element"" style=""{styles}display:flex;flex-direction:column;align-items:center;justify-content:center;background-color:white;border:1px solid #ccc;"">
            <div style=""flex:1;display:flex;align-items:center;"">[BARCODE: {barEl.Data}]</div>
            {(barEl.ShowText ? $"<div style=\"font-size:10px;\">{barEl.Data}</div>" : "")}
        </div>";
                    break;
                    
                case ElementType.QRCode:
                    var qrEl = (QRCodeElement)element;
                    html += $@"
        <div class=""element"" style=""{styles}display:flex;align-items:center;justify-content:center;background-color:white;border:1px solid #ccc;"">
            [QR: {qrEl.Data}]
        </div>";
                    break;
                    
                case ElementType.Image:
                    var imgEl = (ImageElement)element;
                    html += $@"
        <div class=""element"" style=""{styles}background-color:#f0f0f0;display:flex;align-items:center;justify-content:center;"">
            [Image: {System.Net.WebUtility.HtmlEncode(Path.GetFileName(imgEl.ImagePath))}]
        </div>";
                    break;
            }
        }

        html += @"
    </div>
</body>
</html>";

        return html;
    }

    private void OnNewLabelClicked(object sender, EventArgs e)
    {
        CanvasArea.Children.Clear();
        _elementViews.Clear();
        _currentTemplate = new LabelTemplate();
        SetupCanvas();
        UpdateObjectList();
        SelectElement(null);
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Exit", "Are you sure you want to exit?", "Yes", "No");
        if (answer)
        {
            Application.Current?.Quit();
        }
    }

    #endregion
}

public class ObjectListItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
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
        canvas.FillColor = Colors.LightGray;
        canvas.FillRectangle(dirtyRect);
        
        canvas.StrokeColor = Colors.Gray;
        canvas.StrokeSize = 1;
        
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
        canvas.FillColor = Colors.LightGray;
        canvas.FillRectangle(dirtyRect);
        
        canvas.StrokeColor = Colors.Gray;
        canvas.StrokeSize = 1;
        
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
        canvas.StrokeColor = Color.FromArgb("#E0E0E0");
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
