// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using LabelMaker.Models;
using Microsoft.Maui.Graphics;

namespace LabelMaker;

public partial class MainPage
{
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
        if (_selectedElement is not { } el)
            return;

        NoSelectionLabel.IsVisible = false;

        GeneralProperties.IsVisible = true;
        PositionProperties.IsVisible = true;
        AppearanceProperties.IsVisible = true;
        DataBindingProperties.IsVisible = true;

        ElementNameEntry.Text = el.Name;
        VisibleCheckBox.IsChecked = el.IsVisible;
        LockedCheckBox.IsChecked = el.IsLocked;

        PosXEntry.Text = el.X.ToString("F2");
        PosYEntry.Text = el.Y.ToString("F2");
        WidthEntry.Text = el.Width.ToString("F2");
        HeightEntry.Text = el.Height.ToString("F2");
        RotationSlider.Value = el.Rotation;
        RotationEntry.Text = ((int)el.Rotation).ToString();
        OpacitySlider.Value = el.Opacity;
        OpacityValue.Text = $"{(int)(el.Opacity * 100)}%";
        BorderWidthStepper.Value = el.BorderWidth;
        BorderWidthValue.Text = $"{el.BorderWidth:F0} px";
        CornerRadiusStepper.Value = el.CornerRadius;
        CornerRadiusValue.Text = $"{el.CornerRadius:F0} px";
        BorderStylePicker.SelectedIndex = el.BorderStyle switch
        {
            "Solid" => 0,
            "Dashed" => 1,
            "Dotted" => 2,
            "DashDot" => 3,
            "None" => 4,
            _ => 0
        };
        PaddingLeftEntry.Text = el.PaddingLeft.ToString("F0");
        PaddingRightEntry.Text = el.PaddingRight.ToString("F0");
        PaddingTopEntry.Text = el.PaddingTop.ToString("F0");
        PaddingBottomEntry.Text = el.PaddingBottom.ToString("F0");
        DataBindingEntry.Text = el.DataBinding;

        TextProperties.IsVisible = el is TextElement;
        ShapeProperties.IsVisible = el is ShapeElement;
        ImageProperties.IsVisible = el is ImageElement;
        BarcodeProperties.IsVisible = el is BarcodeElement;
        QRCodeProperties.IsVisible = el is QRCodeElement;
        DateTimeProperties.IsVisible = el is DateTimeElement;
        CounterProperties.IsVisible = el is CounterElement;
        DatabaseFieldProperties.IsVisible = el is DatabaseFieldElement;
        RFIDProperties.IsVisible = el is RFIDElement;

        if (el is TextElement textElement)
        {
            TextContentEntry.Text = textElement.Text;
            FontPicker.SelectedItem = textElement.FontFamily;
            FontSizeSlider.Value = textElement.FontSize;
            BoldCheckBox.IsChecked = textElement.Bold;
            ItalicCheckBox.IsChecked = textElement.Italic;
            UnderlineCheckBox.IsChecked = textElement.Underline;
            StrikethroughCheckBox.IsChecked = textElement.Strikethrough;
            AlignmentPicker.SelectedIndex = textElement.HorizontalAlignment switch
            {
                TextAlignment.Start => 0,
                TextAlignment.Center => 1,
                TextAlignment.End => 2,
                _ => 0
            };
            VAlignmentPicker.SelectedIndex = textElement.VerticalAlignment switch
            {
                TextAlignment.Start => 0,
                TextAlignment.Center => 1,
                TextAlignment.End => 2,
                _ => 0
            };
            LineSpacingEntry.Text = textElement.LineSpacing.ToString("F1");
            CharacterSpacingEntry.Text = textElement.CharacterSpacing.ToString("F1");
            TextWrapPicker.SelectedIndex = textElement.TextWrapping switch
            {
                "Wrap" => 0,
                "NoWrap" => 1,
                "Truncate" => 2,
                _ => 0
            };
            TextCasePicker.SelectedIndex = textElement.TextCase switch
            {
                "None" => 0,
                "Upper" => 1,
                "Lower" => 2,
                "Title" => 3,
                _ => 0
            };
            TextOutlineWidthStepper.Value = textElement.TextOutlineWidth;
            TextOutlineWidthValue.Text = textElement.TextOutlineWidth.ToString("F0");
        }

        if (el is ShapeElement shapeElement)
        {
            ShapeFilledCheckBox.IsChecked = shapeElement.Filled;
            ShapeLineStylePicker.SelectedIndex = shapeElement.LineStyle switch
            {
                "Solid" => 0,
                "Dashed" => 1,
                "Dotted" => 2,
                "DashDot" => 3,
                _ => 0
            };
            CornerRadiusStepper.Value = shapeElement.CornerRadius;
        }

        if (el is ImageElement imageElement)
        {
            ImagePathLabel.Text = string.IsNullOrWhiteSpace(imageElement.ImagePath)
                ? "No image selected"
                : Path.GetFileName(imageElement.ImagePath);
            ImageScalingPicker.SelectedIndex = imageElement.ScalingMode switch
            {
                "Uniform" => 0,
                "Fill" => 1,
                "Stretch" => 2,
                "None" => 3,
                _ => 0
            };
            MaintainAspectCheckBox.IsChecked = imageElement.MaintainAspectRatio;
            ImageOpacitySlider.Value = imageElement.Opacity;
            ImageBrightnessSlider.Value = imageElement.Brightness;
            ImageContrastSlider.Value = imageElement.Contrast;
            ImageBrightnessValue.Text = imageElement.Brightness.ToString("F1");
            ImageContrastValue.Text = imageElement.Contrast.ToString("F1");
        }

        if (el is BarcodeElement barcodeElement)
        {
            BarcodePropertiesTitle.Text = "Barcode";
            BarcodeDataEntry.Text = barcodeElement.Data;
            BarcodeTypeLabel.IsVisible = true;
            BarcodeTypePicker.IsVisible = true;
            ShowBarcodeTextOptions.IsVisible = true;
            BarcodeColorLabel.IsVisible = true;
            BarcodeColorOptions.IsVisible = true;
            ShowTextCheckBox.IsChecked = barcodeElement.ShowText;
            BarcodeTextPositionLabel.IsVisible = true;
            BarcodeTextPositionPicker.IsVisible = true;
            BarcodeTextPositionPicker.SelectedIndex = barcodeElement.TextPosition switch
            {
                "Bottom" => 0,
                "Top" => 1,
                "None" => 2,
                _ => 0
            };
            BarcodeDimensionsLabel.IsVisible = true;
            BarcodeDimensionsPanel.IsVisible = true;
            BarWidthStepper.Value = barcodeElement.BarWidth;
            BarHeightStepper.Value = barcodeElement.BarHeight;
            BarcodeChecksumPanel.IsVisible = true;
            IncludeChecksumCheckBox.IsChecked = barcodeElement.IncludeChecksum;
            QuietZoneStepper.Value = barcodeElement.QuietZone;

            var barcodeTypeIndex = barcodeElement.BarcodeType switch
            {
                "CODE128" => 0,
                "CODE39" => 1,
                "EAN13" => 2,
                "EAN8" => 3,
                "UPC-A" => 4,
                "UPC-E" => 5,
                "ITF14" => 6,
                "Codabar" => 7,
                "GS1-128" => 8,
                "Pharmacode" => 9,
                _ => 0
            };
            BarcodeTypePicker.SelectedIndex = barcodeTypeIndex;
        }
        else if (el is QRCodeElement qrCodeElement)
        {
            QRDataEntry.Text = qrCodeElement.Data;
            QRErrorCorrectionPicker.SelectedIndex = qrCodeElement.ErrorCorrection switch
            {
                QRCodeErrorCorrection.Low => 0,
                QRCodeErrorCorrection.Medium => 1,
                QRCodeErrorCorrection.Quartile => 2,
                QRCodeErrorCorrection.High => 3,
                _ => 1
            };
            QRModuleSizeSlider.Value = qrCodeElement.ModuleSize;
            QRModuleSizeValue.Text = qrCodeElement.ModuleSize.ToString("F0");
            QRMarginSlider.Value = qrCodeElement.MarginSize;
            QRMarginValue.Text = qrCodeElement.MarginSize.ToString("F0");

            BarcodePropertiesTitle.Text = "QR Code";
            BarcodeDataEntry.Text = qrCodeElement.Data;
            BarcodeTypeLabel.IsVisible = false;
            BarcodeTypePicker.IsVisible = false;
            ShowBarcodeTextOptions.IsVisible = false;
            BarcodeColorLabel.IsVisible = false;
            BarcodeColorOptions.IsVisible = false;
            BarcodeTextPositionLabel.IsVisible = false;
            BarcodeTextPositionPicker.IsVisible = false;
            BarcodeDimensionsLabel.IsVisible = false;
            BarcodeDimensionsPanel.IsVisible = false;
            BarcodeChecksumPanel.IsVisible = false;
        }

        if (el is DateTimeElement dateTimeElement)
        {
            DateTimeValueTypePicker.SelectedIndex = (int)dateTimeElement.ValueType;
            DateTimeFormatEntry.Text = dateTimeElement.Format;
            DateTimeOffsetEntry.Text = dateTimeElement.OffsetDays.ToString();
            DateTimeFontSizeSlider.Value = dateTimeElement.FontSize;
            DateTimeFontSizeValue.Text = ((int)dateTimeElement.FontSize).ToString();
            DateTimeBoldCheckBox.IsChecked = dateTimeElement.Bold;
        }

        if (el is CounterElement counterElement)
        {
            CounterStartEntry.Text = counterElement.StartValue.ToString();
            CounterStepEntry.Text = counterElement.StepValue.ToString();
            CounterPaddingEntry.Text = counterElement.Padding.ToString();
            CounterPrefixEntry.Text = counterElement.Prefix;
            CounterSuffixEntry.Text = counterElement.Suffix;
            CounterResetModePicker.SelectedIndex = (int)counterElement.ResetMode;
        }

        if (el is DatabaseFieldElement dbFieldElement)
        {
            DbFieldSourceEntry.Text = dbFieldElement.DataSourceId;
            DbFieldNameEntry.Text = dbFieldElement.FieldName;
            DbFieldFormatEntry.Text = dbFieldElement.Format;
            DbFieldFontSizeSlider.Value = dbFieldElement.FontSize;
            DbFieldFontSizeValue.Text = ((int)dbFieldElement.FontSize).ToString();
            DbFieldBoldCheckBox.IsChecked = dbFieldElement.Bold;
        }

        if (el is RFIDElement rfidElement)
        {
            RFIDEpcEntry.Text = rfidElement.EpcValue;
            RFIDMemoryBankPicker.SelectedIndex = rfidElement.MemoryBank switch { "TID" => 1, "User" => 2, "Reserved" => 3, _ => 0 };
            RFIDEncodingPicker.SelectedIndex = rfidElement.EncodingScheme switch { "ISO18000-6B" => 1, "Gen2" => 2, _ => 0 };
            RFIDReadAfterWriteCheckBox.IsChecked = rfidElement.ReadAfterWrite;
            RFIDVoidOnFailureCheckBox.IsChecked = rfidElement.VoidOnFailure;
            RFIDRetryEntry.Text = rfidElement.RetryCount.ToString();
        }
    }

    private void HidePropertiesPanel()
    {
        NoSelectionLabel.IsVisible = true;
        GeneralProperties.IsVisible = false;
        PositionProperties.IsVisible = false;
        AppearanceProperties.IsVisible = false;
        TextProperties.IsVisible = false;
        ShapeProperties.IsVisible = false;
        ImageProperties.IsVisible = false;
        BarcodeProperties.IsVisible = false;
        QRCodeProperties.IsVisible = false;
        DateTimeProperties.IsVisible = false;
        CounterProperties.IsVisible = false;
        DatabaseFieldProperties.IsVisible = false;
        RFIDProperties.IsVisible = false;
        DataBindingProperties.IsVisible = false;
    }

    #endregion

    #region Section Toggle

    private void OnSectionToggleClicked(object sender, EventArgs e)
    {
        if (sender is not Button toggleBtn) return;

        var parent = toggleBtn.Parent;
        while (parent != null && parent is not VerticalStackLayout)
            parent = parent.Parent;

        if (parent is VerticalStackLayout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is VerticalStackLayout inner && inner != layout)
                {
                    var contentPanel = inner.Children.FirstOrDefault(c =>
                        c is VerticalStackLayout vs && vs.AutomationId != "") as VerticalStackLayout;

                    if (contentPanel == null)
                    {
                        for (int i = 0; i < inner.Children.Count; i++)
                        {
                            if (inner.Children[i] is VerticalStackLayout vsInner)
                            {
                                contentPanel = vsInner;
                                break;
                            }
                        }
                    }

                    if (contentPanel != null)
                    {
                        contentPanel.IsVisible = !contentPanel.IsVisible;
                        toggleBtn.Text = contentPanel.IsVisible ? "\u25B2" : "\u25BC";
                        return;
                    }
                }
            }
        }

        var sectionName = toggleBtn.AutomationId ?? "";
        VerticalStackLayout? content = sectionName switch
        {
            "GeneralToggle" => GeneralContent,
            "PositionToggle" => PositionContent,
            "AppearanceToggle" => AppearanceContent,
            "TextToggle" => TextContent,
            "ShapeToggle" => ShapeContent,
            "ImageToggle" => ImageContent,
            "BarcodeToggle" => BarcodeContent,
            "QRCodeToggle" => QRCodeContent,
            "DataBindingToggle" => DataBindingContent,
            _ => null
        };

        if (content != null)
        {
            content.IsVisible = !content.IsVisible;
            toggleBtn.Text = content.IsVisible ? "\u25B2" : "\u25BC";
        }
        else
        {
            var grandparent = toggleBtn.Parent?.Parent;
            if (grandparent is VerticalStackLayout outerLayout)
            {
                var innerContent = outerLayout.Children.OfType<VerticalStackLayout>().LastOrDefault();
                if (innerContent != null)
                {
                    innerContent.IsVisible = !innerContent.IsVisible;
                    toggleBtn.Text = innerContent.IsVisible ? "\u25B2" : "\u25BC";
                }
            }
        }
    }

    #endregion

    #region General Properties

    private void OnElementNameChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.Name = ElementNameEntry.Text ?? "";
        UpdateObjectList();
    }

    private void OnVisibleChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.IsVisible = e.Value;
    }

    private void OnLockedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.IsLocked = e.Value;
    }

    #endregion

    #region Position & Size

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
        RotationEntry.Text = ((int)e.NewValue).ToString();
        UpdateStatusBar();
    }

    private void OnRotationEntryChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        if (double.TryParse(RotationEntry.Text, out double val))
        {
            _selectedElement.Rotation = Math.Clamp(val, 0, 360);
            RotationSlider.Value = _selectedElement.Rotation;
        }
        UpdateStatusBar();
    }

    #endregion

    #region Appearance Properties

    private void OnOpacityChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.Opacity = e.NewValue;
        OpacityValue.Text = $"{(int)(e.NewValue * 100)}%";
    }

    private void OnBorderWidthChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.BorderWidth = e.NewValue;
        BorderWidthValue.Text = $"{e.NewValue:F0} px";
    }

    private void OnCornerRadiusChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.CornerRadius = e.NewValue;
        CornerRadiusValue.Text = $"{e.NewValue:F0} px";
        if (_selectedElement is ShapeElement shape)
            shape.CornerRadius = e.NewValue;
    }

    private void OnBorderStyleChanged(object sender, EventArgs e)
    {
        if (_selectedElement == null || BorderStylePicker.SelectedIndex < 0) return;
        _selectedElement.BorderStyle = BorderStylePicker.SelectedIndex switch
        {
            0 => "Solid",
            1 => "Dashed",
            2 => "Dotted",
            3 => "DashDot",
            4 => "None",
            _ => "Solid"
        };
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

    private async void OnCustomFillColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        var result = await DisplayPromptAsync("Fill Color", "Enter hex color (e.g. #FF5500):", "Apply", "Cancel", "#000000", 7);
        if (result != null && TryParseColor(result, out var color))
            _selectedElement.BackgroundColor = color;
    }

    private async void OnCustomBorderColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        var result = await DisplayPromptAsync("Border Color", "Enter hex color (e.g. #FF5500):", "Apply", "Cancel", "#000000", 7);
        if (result != null && TryParseColor(result, out var color))
            _selectedElement.BorderColor = color;
    }

    private static bool TryParseColor(string hex, out Color color)
    {
        try
        {
            color = Color.FromArgb(hex);
            return true;
        }
        catch
        {
            color = Colors.Black;
            return false;
        }
    }

    private void OnPaddingChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        if (double.TryParse(PaddingLeftEntry.Text, out double pl))
            _selectedElement.PaddingLeft = Math.Max(0, pl);
        if (double.TryParse(PaddingRightEntry.Text, out double pr))
            _selectedElement.PaddingRight = Math.Max(0, pr);
        if (double.TryParse(PaddingTopEntry.Text, out double pt))
            _selectedElement.PaddingTop = Math.Max(0, pt);
        if (double.TryParse(PaddingBottomEntry.Text, out double pb))
            _selectedElement.PaddingBottom = Math.Max(0, pb);
    }

    private void OnFlipHorizontalClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.IsFlippedHorizontal = !_selectedElement.IsFlippedHorizontal;
    }

    private void OnFlipVerticalClicked(object sender, EventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.IsFlippedVertical = !_selectedElement.IsFlippedVertical;
    }

    #endregion

    #region Text Properties

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

    private void OnStrikethroughChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
            textElement.Strikethrough = e.Value;
    }

    private void OnTextColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement is TextElement textElement && sender is Button button)
            textElement.TextColor = button.BackgroundColor;
    }

    private async void OnCustomTextColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement is not TextElement textElement) return;
        var result = await DisplayPromptAsync("Text Color", "Enter hex color (e.g. #FF5500):", "Apply", "Cancel", "#000000", 7);
        if (result != null && TryParseColor(result, out var color))
            textElement.TextColor = color;
    }

    private void OnTextOutlineWidthChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
        {
            textElement.TextOutlineWidth = e.NewValue;
            TextOutlineWidthValue.Text = e.NewValue.ToString("F0");
        }
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

    private void OnVAlignmentChanged(object sender, EventArgs e)
    {
        if (_selectedElement is TextElement textElement && VAlignmentPicker.SelectedIndex >= 0)
        {
            textElement.VerticalAlignment = VAlignmentPicker.SelectedIndex switch
            {
                0 => TextAlignment.Start,
                1 => TextAlignment.Center,
                2 => TextAlignment.End,
                _ => TextAlignment.Start
            };
        }
    }

    private void OnLineSpacingChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement && double.TryParse(LineSpacingEntry.Text, out double val))
            textElement.LineSpacing = val;
    }

    private void OnCharacterSpacingChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement && double.TryParse(CharacterSpacingEntry.Text, out double val))
            textElement.CharacterSpacing = val;
    }

    private void OnTextWrapChanged(object sender, EventArgs e)
    {
        if (_selectedElement is TextElement textElement && TextWrapPicker.SelectedIndex >= 0)
        {
            textElement.TextWrapping = TextWrapPicker.SelectedIndex switch
            {
                0 => "Wrap",
                1 => "NoWrap",
                2 => "Truncate",
                _ => "Wrap"
            };
        }
    }

    private void OnTextCaseChanged(object sender, EventArgs e)
    {
        if (_selectedElement is TextElement textElement && TextCasePicker.SelectedIndex >= 0)
        {
            textElement.TextCase = TextCasePicker.SelectedIndex switch
            {
                0 => "None",
                1 => "Upper",
                2 => "Lower",
                3 => "Title",
                _ => "None"
            };
        }
    }

    #endregion

    #region Shape Properties

    private void OnShapeFilledChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is ShapeElement shape)
            shape.Filled = e.Value;
    }

    private void OnShapeFillColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement is ShapeElement shape && sender is Button button)
            shape.FillColor = button.BackgroundColor;
    }

    private void OnShapeLineStyleChanged(object sender, EventArgs e)
    {
        if (_selectedElement is ShapeElement shape && ShapeLineStylePicker.SelectedIndex >= 0)
        {
            shape.LineStyle = ShapeLineStylePicker.SelectedIndex switch
            {
                0 => "Solid",
                1 => "Dashed",
                2 => "Dotted",
                3 => "DashDot",
                _ => "Solid"
            };
        }
    }

    #endregion

    #region Image Properties

    private async void OnChooseImageClicked(object sender, EventArgs e)
    {
        if (_selectedElement is not ImageElement imageElement)
            return;

        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Choose an image",
                FileTypes = ImageFileTypes
            });

            if (result == null)
                return;

            var localPath = await ImportImageAsync(result);
            imageElement.ImagePath = localPath;
            ImagePathLabel.Text = Path.GetFileName(result.FileName);
            UpdateObjectList();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Image", $"Could not open image: {ex.Message}", "OK");
        }
    }

    private static async Task<string> ImportImageAsync(FileResult result)
    {
        var imageDirectory = Path.Combine(FileSystem.AppDataDirectory, "ImportedImages");
        Directory.CreateDirectory(imageDirectory);

        var extension = Path.GetExtension(result.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".img";

        var localPath = Path.Combine(imageDirectory, $"{Guid.NewGuid():N}{extension}");

        await using var source = await result.OpenReadAsync();
        await using var destination = File.Create(localPath);
        await source.CopyToAsync(destination);

        return localPath;
    }

    private static readonly FilePickerFileType ImageFileTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, new[] { "public.image" } },
        { DevicePlatform.MacCatalyst, new[] { "public.image" } },
        { DevicePlatform.Android, new[] { "image/*" } },
        { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" } }
    });

    private void OnImageScalingChanged(object sender, EventArgs e)
    {
        if (_selectedElement is ImageElement imageElement && ImageScalingPicker.SelectedIndex >= 0)
        {
            imageElement.ScalingMode = ImageScalingPicker.SelectedIndex switch
            {
                0 => "Uniform",
                1 => "Fill",
                2 => "Stretch",
                3 => "None",
                _ => "Uniform"
            };
        }
    }

    private void OnMaintainAspectChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is ImageElement imageElement)
            imageElement.MaintainAspectRatio = e.Value;
    }

    private void OnImageOpacityChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is ImageElement imageElement)
            imageElement.Opacity = e.NewValue;
    }

    private void OnImageBrightnessChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is ImageElement imageElement)
        {
            imageElement.Brightness = e.NewValue;
            ImageBrightnessValue.Text = e.NewValue.ToString("F1");
        }
    }

    private void OnImageContrastChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is ImageElement imageElement)
        {
            imageElement.Contrast = e.NewValue;
            ImageContrastValue.Text = e.NewValue.ToString("F1");
        }
    }

    #endregion

    #region Barcode Properties

    private void OnBarcodeDataChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement)
        {
            barcodeElement.Data = BarcodeDataEntry.Text;
            UpdateObjectList();
        }
        else if (_selectedElement is QRCodeElement qrCodeElement)
        {
            qrCodeElement.Data = QRDataEntry.Text;
            UpdateObjectList();
        }
    }

    private void OnQRDataChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is QRCodeElement qrCodeElement)
        {
            qrCodeElement.Data = QRDataEntry.Text;
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

    private void OnBarcodeTextPositionChanged(object sender, EventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement && BarcodeTextPositionPicker.SelectedIndex >= 0)
        {
            barcodeElement.TextPosition = BarcodeTextPositionPicker.SelectedIndex switch
            {
                0 => "Bottom",
                1 => "Top",
                2 => "None",
                _ => "Bottom"
            };
        }
    }

    private void OnBarWidthChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement)
            barcodeElement.BarWidth = e.NewValue;
    }

    private void OnBarHeightChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement)
            barcodeElement.BarHeight = e.NewValue;
    }

    private void OnIncludeChecksumChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement)
            barcodeElement.IncludeChecksum = e.Value;
    }

    private void OnQuietZoneChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is BarcodeElement barcodeElement)
            barcodeElement.QuietZone = e.NewValue;
    }

    #endregion

    #region QR Code Properties

    private void OnQRErrorCorrectionChanged(object sender, EventArgs e)
    {
        if (_selectedElement is QRCodeElement qrCodeElement && QRErrorCorrectionPicker.SelectedIndex >= 0)
        {
            qrCodeElement.ErrorCorrection = QRErrorCorrectionPicker.SelectedIndex switch
            {
                0 => QRCodeErrorCorrection.Low,
                1 => QRCodeErrorCorrection.Medium,
                2 => QRCodeErrorCorrection.Quartile,
                3 => QRCodeErrorCorrection.High,
                _ => QRCodeErrorCorrection.Medium
            };
        }
    }

    private void OnQRColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement is QRCodeElement qrCodeElement && sender is Button button)
            qrCodeElement.QRColor = button.BackgroundColor;
    }

    private void OnQRBgColorClicked(object sender, EventArgs e)
    {
        if (_selectedElement is QRCodeElement qrCodeElement && sender is Button button)
            qrCodeElement.QRBackgroundColor = button.BackgroundColor;
    }

    private void OnQRModuleSizeChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is QRCodeElement qrCodeElement)
        {
            qrCodeElement.ModuleSize = e.NewValue;
            QRModuleSizeValue.Text = e.NewValue.ToString("F0");
        }
    }

    private void OnQRMarginChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is QRCodeElement qrCodeElement)
        {
            qrCodeElement.MarginSize = e.NewValue;
            QRMarginValue.Text = e.NewValue.ToString("F0");
        }
    }

    #endregion

    #region Data Binding

    private void OnDataBindingChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        _selectedElement.DataBinding = DataBindingEntry.Text ?? "";
    }

    #endregion

    #region DateTime Properties

    private void OnDateTimeValueTypeChanged(object sender, EventArgs e)
    {
        if (_selectedElement is not DateTimeElement dt) return;
        if (DateTimeValueTypePicker.SelectedIndex >= 0)
            dt.ValueType = (DateTimeValueType)DateTimeValueTypePicker.SelectedIndex;
    }

    private void OnDateTimeFormatChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not DateTimeElement dt) return;
        dt.Format = DateTimeFormatEntry.Text ?? "dd-MM-yyyy";
    }

    private void OnDateTimeOffsetChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not DateTimeElement dt) return;
        if (int.TryParse(DateTimeOffsetEntry.Text, out var offset))
            dt.OffsetDays = offset;
    }

    private void OnDateTimeFontSizeChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is not DateTimeElement dt) return;
        dt.FontSize = e.NewValue;
        DateTimeFontSizeValue.Text = ((int)e.NewValue).ToString();
    }

    private void OnDateTimeBoldChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is not DateTimeElement dt) return;
        dt.Bold = e.Value;
    }

    #endregion

    #region Counter Properties

    private void OnCounterStartChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not CounterElement c) return;
        if (long.TryParse(CounterStartEntry.Text, out var val))
            c.StartValue = val;
    }

    private void OnCounterStepChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not CounterElement c) return;
        if (long.TryParse(CounterStepEntry.Text, out var val))
            c.StepValue = val;
    }

    private void OnCounterPaddingChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not CounterElement c) return;
        if (int.TryParse(CounterPaddingEntry.Text, out var val))
            c.Padding = val;
    }

    private void OnCounterPrefixChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not CounterElement c) return;
        c.Prefix = CounterPrefixEntry.Text ?? "";
    }

    private void OnCounterSuffixChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not CounterElement c) return;
        c.Suffix = CounterSuffixEntry.Text ?? "";
    }

    private void OnCounterResetModeChanged(object sender, EventArgs e)
    {
        if (_selectedElement is not CounterElement c) return;
        if (CounterResetModePicker.SelectedIndex >= 0)
            c.ResetMode = (CounterResetMode)CounterResetModePicker.SelectedIndex;
    }

    #endregion

    #region DatabaseField Properties

    private void OnDbFieldSourceChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not DatabaseFieldElement db) return;
        db.DataSourceId = DbFieldSourceEntry.Text ?? "";
    }

    private void OnDbFieldNameChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not DatabaseFieldElement db) return;
        db.FieldName = DbFieldNameEntry.Text ?? "";
    }

    private void OnDbFieldFormatChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not DatabaseFieldElement db) return;
        db.Format = DbFieldFormatEntry.Text ?? "";
    }

    private void OnDbFieldFontSizeChanged(object sender, ValueChangedEventArgs e)
    {
        if (_selectedElement is not DatabaseFieldElement db) return;
        db.FontSize = e.NewValue;
        DbFieldFontSizeValue.Text = ((int)e.NewValue).ToString();
    }

    private void OnDbFieldBoldChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is not DatabaseFieldElement db) return;
        db.Bold = e.Value;
    }

    #endregion

    #region RFID Properties

    private void OnRFIDEpcChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not RFIDElement rfid) return;
        rfid.EpcValue = RFIDEpcEntry.Text ?? "";
    }

    private void OnRFIDMemoryBankChanged(object sender, EventArgs e)
    {
        if (_selectedElement is not RFIDElement rfid) return;
        if (RFIDMemoryBankPicker.SelectedIndex >= 0)
            rfid.MemoryBank = RFIDMemoryBankPicker.ItemsSource[RFIDMemoryBankPicker.SelectedIndex]?.ToString() ?? "EPC";
    }

    private void OnRFIDEncodingChanged(object sender, EventArgs e)
    {
        if (_selectedElement is not RFIDElement rfid) return;
        if (RFIDEncodingPicker.SelectedIndex >= 0)
            rfid.EncodingScheme = RFIDEncodingPicker.ItemsSource[RFIDEncodingPicker.SelectedIndex]?.ToString() ?? "ISO18000-6C";
    }

    private void OnRFIDReadAfterWriteChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is not RFIDElement rfid) return;
        rfid.ReadAfterWrite = e.Value;
    }

    private void OnRFIDVoidOnFailureChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_selectedElement is not RFIDElement rfid) return;
        rfid.VoidOnFailure = e.Value;
    }

    private void OnRFIDRetryChanged(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is not RFIDElement rfid) return;
        if (int.TryParse(RFIDRetryEntry.Text, out var val))
            rfid.RetryCount = val;
    }

    #endregion
}