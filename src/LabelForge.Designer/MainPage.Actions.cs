// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Text.Json;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using LabelMaker.Models;

namespace LabelMaker;

public partial class MainPage
{
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

    private void OnShowGridClicked(object sender, EventArgs e)
    {
        GridCheckBox.IsChecked = !GridCheckBox.IsChecked;
        GridBackground.IsVisible = GridCheckBox.IsChecked;
    }

    private void OnZoomResetClicked(object sender, EventArgs e)
    {
        _zoomLevel = 1.0;
        ApplyZoom();
    }

    private void OnNudgeLeftClicked(object sender, EventArgs e)
    {
        NudgeSelectedElement(-1, 0);
    }

    private void OnNudgeRightClicked(object sender, EventArgs e)
    {
        NudgeSelectedElement(1, 0);
    }

    private void OnNudgeUpClicked(object sender, EventArgs e)
    {
        NudgeSelectedElement(0, -1);
    }

    private void OnNudgeDownClicked(object sender, EventArgs e)
    {
        NudgeSelectedElement(0, 1);
    }

    private void NudgeSelectedElement(double deltaX, double deltaY)
    {
        if (_selectedElement == null) return;
        RecordUndoState();
        _selectedElement.X += deltaX;
        _selectedElement.Y += deltaY;
        if (_elementViews.TryGetValue(_selectedElement, out View? container))
        {
            UpdateElementPosition(_selectedElement, container);
        }
        UpdatePropertiesPanel();
        UpdateStatusBar();
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
            RecordUndoState();
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
        if (!string.IsNullOrEmpty(_currentFilePath))
        {
            try
            {
                var json = JsonSerializer.Serialize(_currentTemplate, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await File.WriteAllTextAsync(_currentFilePath, json);
                _isModified = false;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Save Error", $"Could not save file:\n{ex.Message}", "OK");
            }
        }
        else
        {
            await SaveAsAsync();
        }
    }

    private void LoadTemplate(LabelTemplate template, bool recordUndo = true)
    {
        if (recordUndo)
            RecordUndoState();
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
        await Navigation.PushAsync(new PrintDialogPage(_currentTemplate));
    }

    private void OnNewLabelClicked(object sender, EventArgs e)
    {
        RecordUndoState();
        CanvasArea.Children.Clear();
        _elementViews.Clear();
        _currentTemplate = new LabelTemplate();
        _currentFilePath = null;
        _isModified = false;
        SetupCanvas();
        UpdateObjectList();
        SelectElement(null);
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        var fileName = string.IsNullOrEmpty(_currentFilePath)
            ? "Untitled"
            : Path.GetFileNameWithoutExtension(_currentFilePath);
        var modified = _isModified ? " *" : "";
        Title = $"{fileName}{modified} - Label Maker";
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Exit", "Are you sure you want to exit?", "Yes", "No");
        if (answer)
        {
            #if MACCATALYST || IOS
            Environment.Exit(0);
            #else
            Application.Current?.Quit();
            #endif
        }
    }

    #endregion

    #region Undo/Redo

    private void RecordUndoState()
    {
        if (_isApplyingUndo) return;
        var json = JsonSerializer.Serialize(_currentTemplate, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        _undoStack.Push(json);
        _redoStack.Clear();
        _isModified = true;
        UpdateTitle();
    }

    private void ApplyUndoState(string json)
    {
        var template = JsonSerializer.Deserialize<LabelTemplate>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        if (template != null)
        {
            _isApplyingUndo = true;
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
            _isApplyingUndo = false;
        }
    }

    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (_undoStack.Count == 0)
        {
            DisplayAlert("Undo", "Nothing to undo.", "OK");
            return;
        }
        var currentJson = JsonSerializer.Serialize(_currentTemplate, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        _redoStack.Push(currentJson);
        var previousJson = _undoStack.Pop();
        ApplyUndoState(previousJson);
    }

    private void OnRedoClicked(object sender, EventArgs e)
    {
        if (_redoStack.Count == 0)
        {
            DisplayAlert("Redo", "Nothing to redo.", "OK");
            return;
        }
        var currentJson = JsonSerializer.Serialize(_currentTemplate, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        _undoStack.Push(currentJson);
        var nextJson = _redoStack.Pop();
        ApplyUndoState(nextJson);
    }

    #endregion

    #region Clipboard

    private static LabelElement DeepCloneElement(LabelElement source)
    {
        LabelElement clone = source switch
        {
            TextElement t => new TextElement
            {
                Text = t.Text,
                FontFamily = t.FontFamily,
                FontSize = t.FontSize,
                Bold = t.Bold,
                Italic = t.Italic,
                Underline = t.Underline,
                TextColor = t.TextColor,
                HorizontalAlignment = t.HorizontalAlignment,
                VerticalAlignment = t.VerticalAlignment
            },
            ShapeElement s => new ShapeElement(s.Type)
            {
                Filled = s.Filled
            },
            ImageElement i => new ImageElement
            {
                ImagePath = i.ImagePath
            },
            BarcodeElement b => new BarcodeElement
            {
                Data = b.Data,
                BarcodeType = b.BarcodeType,
                ShowText = b.ShowText,
                BarcodeColor = b.BarcodeColor
            },
            QRCodeElement q => new QRCodeElement
            {
                Data = q.Data,
                QRColor = q.QRColor
            },
            _ => new LabelElement()
        };

        clone.X = source.X;
        clone.Y = source.Y;
        clone.Width = source.Width;
        clone.Height = source.Height;
        clone.Rotation = source.Rotation;
        clone.IsFlippedHorizontal = source.IsFlippedHorizontal;
        clone.IsFlippedVertical = source.IsFlippedVertical;
        clone.BackgroundColor = source.BackgroundColor;
        clone.BorderColor = source.BorderColor;
        clone.BorderWidth = source.BorderWidth;

        return clone;
    }

    private void OnCutClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        RecordUndoState();
        _clipboardElement = DeepCloneElement(_selectedElement);
        OnDeleteClicked(sender, e);
    }

    private void OnCopyClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        _clipboardElement = DeepCloneElement(_selectedElement);
    }

    private void OnPasteClicked(object sender, EventArgs e)
    {
        if (_clipboardElement == null) return;
        RecordUndoState();
        var clone = DeepCloneElement(_clipboardElement);
        clone.X += 20;
        clone.Y += 20;
        _currentTemplate.Elements.Add(clone);
        CreateVisualElement(clone);
        UpdateObjectList();
        SelectElement(clone);
    }

    private void OnSelectAllClicked(object sender, EventArgs e)
    {
        foreach (var element in _currentTemplate.Elements)
        {
            element.IsSelected = true;
        }
        if (_currentTemplate.Elements.Count > 0)
        {
            SelectElement(_currentTemplate.Elements.Last());
        }
    }

    #endregion

    #region File Operations

    private async void OnSaveAsClicked(object sender, EventArgs e)
    {
        await SaveAsAsync();
    }

    private async Task SaveAsAsync()
    {
        try
        {
            var defaultName = _currentTemplate.Name.Replace(" ", "_");
            if (!defaultName.EndsWith(".btw", StringComparison.OrdinalIgnoreCase))
                defaultName += ".btw";

            var json = JsonSerializer.Serialize(_currentTemplate, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var fileName = await DisplayPromptAsync("Save As", "Enter file name:", "Save", "Cancel", defaultName);
            if (string.IsNullOrWhiteSpace(fileName)) return;

            if (!fileName.EndsWith(".btw", StringComparison.OrdinalIgnoreCase))
                fileName += ".btw";

            var path = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
            await File.WriteAllTextAsync(path, json);

            _currentFilePath = path;
            _isModified = false;
            UpdateTitle();
        }
        catch (OperationCanceledException)
        {
            // User cancelled
        }
        catch (Exception ex)
        {
            await DisplayAlert("Save Error", $"Could not save file:\n{ex.Message}", "OK");
        }
    }

    private async void OnLoadClicked(object sender, EventArgs e)
    {
        if (_isModified)
        {
            var confirm = await DisplayAlert("Unsaved Changes",
                "Current label has unsaved changes. Discard and open another?", "Discard", "Cancel");
            if (!confirm) return;
        }

        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".btw" } },
                { DevicePlatform.MacCatalyst, new[] { "public.json", "public.data" } },
                { DevicePlatform.iOS, new[] { "public.json", "public.data" } },
                { DevicePlatform.Android, new[] { "application/json" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Open Label File",
                FileTypes = customFileType
            });

            if (result != null)
            {
                var json = await File.ReadAllTextAsync(result.FullPath);
                var template = JsonSerializer.Deserialize<LabelTemplate>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (template != null)
                {
                    _currentFilePath = result.FullPath;
                    _isModified = false;
                    LoadTemplate(template, true);
                    UpdateTitle();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // User cancelled
        }
        catch (Exception ex)
        {
            await DisplayAlert("Open Error", $"Could not open file:\n{ex.Message}", "OK");
        }
    }

    #endregion

    #region Arrange Operations

    private List<LabelElement> GetSelectedElements()
    {
        return _currentTemplate.Elements.Where(el => el.IsSelected).ToList();
    }

    private void OnBringToFrontClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        RecordUndoState();
        var element = _selectedElement;
        _currentTemplate.Elements.Remove(element);
        _currentTemplate.Elements.Add(element);
        if (_elementViews.TryGetValue(element, out View? view))
        {
            CanvasArea.Children.Remove(view);
            CanvasArea.Children.Add(view);
        }
        UpdateObjectList();
    }

    private void OnSendToBackClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        RecordUndoState();
        var element = _selectedElement;
        _currentTemplate.Elements.Remove(element);
        _currentTemplate.Elements.Insert(0, element);
        if (_elementViews.TryGetValue(element, out View? view))
        {
            CanvasArea.Children.Remove(view);
            CanvasArea.Children.Insert(0, view);
        }
        UpdateObjectList();
    }

    private void OnBringForwardClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        RecordUndoState();
        var element = _selectedElement;
        var index = _currentTemplate.Elements.IndexOf(element);
        if (index < _currentTemplate.Elements.Count - 1)
        {
            _currentTemplate.Elements.RemoveAt(index);
            _currentTemplate.Elements.Insert(index + 1, element);
            if (_elementViews.TryGetValue(element, out View? view))
            {
                CanvasArea.Children.Remove(view);
                CanvasArea.Children.Insert(index + 1, view);
            }
        }
        UpdateObjectList();
    }

    private void OnSendBackwardClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        RecordUndoState();
        var element = _selectedElement;
        var index = _currentTemplate.Elements.IndexOf(element);
        if (index > 0)
        {
            _currentTemplate.Elements.RemoveAt(index);
            _currentTemplate.Elements.Insert(index - 1, element);
            if (_elementViews.TryGetValue(element, out View? view))
            {
                CanvasArea.Children.Remove(view);
                CanvasArea.Children.Insert(index - 1, view);
            }
        }
        UpdateObjectList();
    }

    private void OnAlignLeftClicked(object sender, EventArgs e)
    {
        var selected = GetSelectedElements();
        if (selected.Count < 2) return;
        RecordUndoState();
        var reference = selected.First().X;
        foreach (var element in selected)
            element.X = reference;
    }

    private void OnAlignCenterClicked(object sender, EventArgs e)
    {
        var selected = GetSelectedElements();
        if (selected.Count < 2) return;
        RecordUndoState();
        var reference = selected.First().X + selected.First().Width / 2;
        foreach (var element in selected)
            element.X = reference - element.Width / 2;
    }

    private void OnAlignRightClicked(object sender, EventArgs e)
    {
        var selected = GetSelectedElements();
        if (selected.Count < 2) return;
        RecordUndoState();
        var reference = selected.First().X + selected.First().Width;
        foreach (var element in selected)
            element.X = reference - element.Width;
    }

    private void OnAlignTopClicked(object sender, EventArgs e)
    {
        var selected = GetSelectedElements();
        if (selected.Count < 2) return;
        RecordUndoState();
        var reference = selected.First().Y;
        foreach (var element in selected)
            element.Y = reference;
    }

    private void OnAlignMiddleClicked(object sender, EventArgs e)
    {
        var selected = GetSelectedElements();
        if (selected.Count < 2) return;
        RecordUndoState();
        var reference = selected.First().Y + selected.First().Height / 2;
        foreach (var element in selected)
            element.Y = reference - element.Height / 2;
    }

    private void OnAlignBottomClicked(object sender, EventArgs e)
    {
        var selected = GetSelectedElements();
        if (selected.Count < 2) return;
        RecordUndoState();
        var reference = selected.First().Y + selected.First().Height;
        foreach (var element in selected)
            element.Y = reference - element.Height;
    }

    private void OnDistributeHorizontallyClicked(object sender, EventArgs e)
    {
        var selected = GetSelectedElements();
        if (selected.Count < 3) return;
        RecordUndoState();
        var sorted = selected.OrderBy(el => el.X).ToList();
        var totalWidth = sorted.Last().X - sorted.First().X;
        var count = sorted.Count;
        var step = totalWidth / (count - 1);
        for (int i = 0; i < count; i++)
            sorted[i].X = sorted.First().X + step * i;
    }

    private void OnDistributeVerticallyClicked(object sender, EventArgs e)
    {
        var selected = GetSelectedElements();
        if (selected.Count < 3) return;
        RecordUndoState();
        var sorted = selected.OrderBy(el => el.Y).ToList();
        var totalHeight = sorted.Last().Y - sorted.First().Y;
        var count = sorted.Count;
        var step = totalHeight / (count - 1);
        for (int i = 0; i < count; i++)
            sorted[i].Y = sorted.First().Y + step * i;
    }

    private void OnGroupClicked(object sender, EventArgs e)
    {
        DisplayAlert("Group", "Group functionality is not yet implemented.", "OK");
    }

    private void OnUngroupClicked(object sender, EventArgs e)
    {
        DisplayAlert("Ungroup", "Ungroup functionality is not yet implemented.", "OK");
    }

    private void OnRotate90CWClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        RecordUndoState();
        _selectedElement.Rotation = NormalizeDegrees(_selectedElement.Rotation + 90);
        RotationSlider.Value = _selectedElement.Rotation;
        RotationEntry.Text = $"{_selectedElement.Rotation:F0}";
        UpdateStatusBar();
    }

    private void OnRotate90CCWClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        RecordUndoState();
        _selectedElement.Rotation = NormalizeDegrees(_selectedElement.Rotation - 90);
        RotationSlider.Value = _selectedElement.Rotation;
        RotationEntry.Text = $"{_selectedElement.Rotation:F0}";
        UpdateStatusBar();
    }

    #endregion

    #region View Operations

    private void OnZoomToFitClicked(object sender, EventArgs e)
    {
        var canvasWidth = _currentTemplate.Width + 64;
        var canvasHeight = _currentTemplate.Height + 64;
        var availableWidth = CanvasScrollView.Width;
        var availableHeight = CanvasScrollView.Height;
        if (availableWidth <= 0 || availableHeight <= 0)
        {
            availableWidth = 800;
            availableHeight = 600;
        }
        var zoomX = availableWidth / canvasWidth;
        var zoomY = availableHeight / canvasHeight;
        _zoomLevel = Math.Max(0.3, Math.Min(3.0, Math.Min(zoomX, zoomY)));
        ApplyZoom();
    }

    private void OnShowRulersClicked(object sender, EventArgs e)
    {
        _rulersVisible = !_rulersVisible;
        var grid = CanvasContainer.Parent?.Parent as Grid;
        if (grid != null)
        {
            foreach (var child in grid.Children)
            {
                if (child is View view)
                {
                    var row = Microsoft.Maui.Controls.Grid.GetRow(view);
                    var col = Microsoft.Maui.Controls.Grid.GetColumn(view);
                    if ((row == 0 && col == 1) || (row == 1 && col == 0) || (row == 1 && col == 2) || (row == 2 && col == 1))
                        view.IsVisible = _rulersVisible;
                }
            }
        }
    }

    private void OnShowObjectTreeClicked(object sender, EventArgs e)
    {
        _objectTreeVisible = !_objectTreeVisible;
        var mainGrid = CanvasContainer?.Parent?.Parent?.Parent?.Parent as Grid;
        if (mainGrid != null && mainGrid.ColumnDefinitions.Count >= 3)
        {
            mainGrid.ColumnDefinitions[0].Width = _objectTreeVisible ? new GridLength(220) : new GridLength(0);
        }
    }

    private void OnShowPropertiesClicked(object sender, EventArgs e)
    {
        _propertiesVisible = !_propertiesVisible;
        var mainGrid = CanvasContainer?.Parent?.Parent?.Parent?.Parent as Grid;
        if (mainGrid != null && mainGrid.ColumnDefinitions.Count >= 3)
        {
            mainGrid.ColumnDefinitions[2].Width = _propertiesVisible ? new GridLength(300) : new GridLength(0);
        }
    }

    private void OnFullscreenClicked(object sender, EventArgs e)
    {
        var window = Application.Current?.Windows[0];
        if (window != null)
        {
            try
            {
                window.MaximumWidth = DeviceDisplay.MainDisplayInfo.Width;
                window.MaximumHeight = DeviceDisplay.MainDisplayInfo.Height;
            }
            catch { }
        }
    }

    #endregion

    #region Printer Operations

    private async void OnPrintPreviewClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PrintDialogPage(_currentTemplate));
    }

    private async void OnPageSetupClicked(object sender, EventArgs e)
    {
        var width = await DisplayPromptAsync("Page Setup", "Enter page width (mm):", "OK", "Cancel", "210");
        var height = await DisplayPromptAsync("Page Setup", "Enter page height (mm):", "OK", "Cancel", "297");
        if (double.TryParse(width, out double w) && double.TryParse(height, out double h))
        {
            _currentTemplate.Width = w * 3.78;
            _currentTemplate.Height = h * 3.78;
            SetupCanvas();
        }
    }

    private async void OnLabelSetupClicked(object sender, EventArgs e)
    {
        var width = await DisplayPromptAsync("Label Setup", "Enter label width (px):", "OK", "Cancel", _currentTemplate.Width.ToString("F0"));
        var height = await DisplayPromptAsync("Label Setup", "Enter label height (px):", "OK", "Cancel", _currentTemplate.Height.ToString("F0"));
        if (double.TryParse(width, out double w) && double.TryParse(height, out double h))
        {
            RecordUndoState();
            _currentTemplate.Width = w;
            _currentTemplate.Height = h;
            SetupCanvas();
        }
    }

    private async void OnSelectPrinterClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Select Printer", "Printer selection is not yet implemented.", "OK");
    }

    private async void OnPrinterPropertiesClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Printer Properties", "Printer properties are not yet implemented.", "OK");
    }

    #endregion

    #region Help Operations

    private async void OnDocumentationClicked(object sender, EventArgs e)
    {
        var helpText = "Label Maker Help\n\n" +
            "Create labels with text, shapes, images, barcodes, and QR codes.\n" +
            "Use the toolbar or menu to add elements.\n" +
            "Drag elements to move them.\n" +
            "Use the properties panel to edit selected elements.\n" +
            "Use Arrange menu for alignment and ordering.\n" +
            "Save and load labels from the File menu.\n" +
            "Print labels from the File or Printer menu.";
        await DisplayAlert("Documentation", helpText, "OK");
    }

    private async void OnKeyboardShortcutsClicked(object sender, EventArgs e)
    {
        var shortcuts = "Keyboard Shortcuts\n\n" +
            "Ctrl+N: New Label\n" +
            "Ctrl+O: Open Label\n" +
            "Ctrl+S: Save Label\n" +
            "Ctrl+Z: Undo\n" +
            "Ctrl+Shift+Z: Redo\n" +
            "Ctrl+X: Cut\n" +
            "Ctrl+C: Copy\n" +
            "Ctrl+V: Paste\n" +
            "Delete: Delete Selected\n" +
            "Ctrl+A: Select All\n" +
            "Ctrl+G: Toggle Grid\n" +
            "Ctrl+Plus: Zoom In\n" +
            "Ctrl+Minus: Zoom Out\n" +
            "Ctrl+0: Zoom 100%\n" +
            "Arrow Keys: Nudge\n" +
            "Ctrl+Shift+Right: Bring to Front\n" +
            "Ctrl+Shift+Left: Send to Back";
        await DisplayAlert("Keyboard Shortcuts", shortcuts, "OK");
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        var aboutText = "Label Maker v1.0\n\n" +
            "A cross-platform label design application built with .NET MAUI.\n\n" +
            "Author: Debarun Lahiri\n" +
            "GitHub: https://github.com/debarunlahiri/\n\n" +
            "Supported Platforms: macOS, Windows, iOS, Android";
        await DisplayAlert("About Label Maker", aboutText, "OK");
    }

    #endregion

    #region Preferences

    private async void OnPreferencesClicked(object sender, EventArgs e)
    {
        var prefs = "Preferences\n\n" +
            "Grid Size: 20px (default)\n" +
            "Snap to Grid: Not yet implemented\n" +
            "Default Font: Arial\n" +
            "Default Label Size: 400x300 px\n" +
            "Auto-save: Not yet implemented\n\n" +
            "These preferences will be configurable in a future update.";
        await DisplayAlert("Preferences", prefs, "OK");
    }

    #endregion
}