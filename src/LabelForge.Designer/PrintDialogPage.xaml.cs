// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using LabelMaker.Models;
using LabelMaker.Services;
using LabelMaker.Services.Printing;

namespace LabelMaker;

public partial class PrintDialogPage : ContentPage
{
    private readonly LabelTemplate _template;
    private readonly PrinterManager _printerManager;
    private readonly LabelPrinterGenerator _labelGenerator;
    private List<PrinterInfo> _printers = new();
    private PrinterInfo? _selectedPrinter;

    public PrintDialogPage(LabelTemplate template)
    {
        InitializeComponent();
        _template = template;
        _printerManager = new PrinterManager();
        _labelGenerator = new LabelPrinterGenerator();
        
        // Wire up events
        DarknessSlider.ValueChanged += OnDarknessChanged;
        SpeedSlider.ValueChanged += OnSpeedChanged;
        
        LoadPrinters();
    }

    private async void LoadPrinters()
    {
        try
        {
            _printers = await _printerManager.GetPrintersAsync();
            
            Device.BeginInvokeOnMainThread(() =>
            {
                PrinterPicker.ItemsSource = _printers.Select(p => p.Name).ToList();
                
                // Select default printer if available
                var defaultPrinter = _printers.FirstOrDefault(p => p.IsDefault);
                if (defaultPrinter != null)
                {
                    PrinterPicker.SelectedIndex = _printers.IndexOf(defaultPrinter);
                }
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load printers: {ex.Message}", "OK");
        }
    }

    private void OnPrinterSelected(object sender, EventArgs e)
    {
        if (PrinterPicker.SelectedIndex >= 0 && PrinterPicker.SelectedIndex < _printers.Count)
        {
            _selectedPrinter = _printers[PrinterPicker.SelectedIndex];
            UpdatePrinterUI();
        }
    }

    private void UpdatePrinterUI()
    {
        if (_selectedPrinter == null) return;
        
        // Show printer info
        PrinterInfoFrame.IsVisible = true;
        PrinterTypeLabel.Text = _selectedPrinter.Type.ToString();
        PrinterPortLabel.Text = _selectedPrinter.Port ?? "Default";
        PrinterLanguageLabel.Text = _selectedPrinter.IsLabelPrinter ? _selectedPrinter.LabelPrinterLanguage : "Standard";
        
        // Show label printer options if it's a label printer
        LabelPrinterOptions.IsVisible = _selectedPrinter.IsLabelPrinter;
        NetworkPrinterFrame.IsVisible = _selectedPrinter.IsNetwork || _selectedPrinter.Port == "9100";
        
        // Set language picker based on printer
        if (_selectedPrinter.IsLabelPrinter)
        {
            var langIndex = _selectedPrinter.LabelPrinterLanguage switch
            {
                "EPL" => 1,
                "CPCL" => 2,
                _ => 0
            };
            LanguagePicker.SelectedIndex = langIndex;
        }
    }

    private async void OnRefreshPrintersClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Refreshing", "Scanning for printers...", "OK");
        LoadPrinters();
    }

    private void OnCopiesChanged(object sender, ValueChangedEventArgs e)
    {
        CopiesLabel.Text = $"{(int)e.NewValue} copy{(e.NewValue > 1 ? "ies" : "y")}";
    }

    private void OnDarknessChanged(object sender, ValueChangedEventArgs e)
    {
        DarknessValue.Text = ((int)e.NewValue).ToString();
    }

    private void OnSpeedChanged(object sender, ValueChangedEventArgs e)
    {
        SpeedValue.Text = ((int)e.NewValue).ToString();
    }

    private async void OnPrintClicked(object sender, EventArgs e)
    {
        if (_selectedPrinter == null)
        {
            await DisplayAlert("No Printer", "Please select a printer first.", "OK");
            return;
        }

        var options = GetPrintOptions();
        
        try
        {
            bool success = false;
            
            if (OutputFormatPicker.SelectedIndex == 0) // HTML
            {
                success = await _printerManager.PrintAsync(_template, options);
            }
            else if (OutputFormatPicker.SelectedIndex == 1) // Raw ZPL
            {
                var zpl = _labelGenerator.GenerateZpl(_template, options);
                success = await _printerManager.PrintZplAsync(zpl, options);
            }
            else if (OutputFormatPicker.SelectedIndex == 2) // Raw EPL
            {
                var epl = _labelGenerator.GenerateEpl(_template, options);
                success = await _printerManager.PrintEplAsync(epl, options);
            }
            else if (OutputFormatPicker.SelectedIndex == 3) // Raw CPCL
            {
                var cpcl = _labelGenerator.GenerateCpcl(_template, options);
                success = await _printerManager.PrintRawAsync(System.Text.Encoding.UTF8.GetBytes(cpcl), options);
            }
            
            if (success)
            {
                await DisplayAlert("Success", "Print job sent successfully.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to send print job. Check printer connection.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Print failed: {ex.Message}", "OK");
        }
    }

    private async void OnPreviewClicked(object sender, EventArgs e)
    {
        try
        {
            var options = GetPrintOptions();
            var htmlService = new PrintService();
            var html = await htmlService.GeneratePrintHtmlAsync(_template, new PrintSettings
            {
                Copies = options.Copies,
                PaperSize = options.PaperSize,
                Orientation = options.Orientation.ToString()
            });
            
            var filePath = Path.Combine(FileSystem.Current.CacheDirectory, "preview.html");
            await File.WriteAllTextAsync(filePath, html);
            await Launcher.OpenAsync(new OpenFileRequest("Print Preview", new ReadOnlyFile(filePath)));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Preview failed: {ex.Message}", "OK");
        }
    }

    private async void OnViewRawOutputClicked(object sender, EventArgs e)
    {
        var options = GetPrintOptions();
        string rawOutput = "";
        string fileExtension = "";
        
        switch (OutputFormatPicker.SelectedIndex)
        {
            case 1: // ZPL
                rawOutput = _labelGenerator.GenerateZpl(_template, options);
                fileExtension = "zpl";
                break;
            case 2: // EPL
                rawOutput = _labelGenerator.GenerateEpl(_template, options);
                fileExtension = "epl";
                break;
            case 3: // CPCL
                rawOutput = _labelGenerator.GenerateCpcl(_template, options);
                fileExtension = "cpcl";
                break;
            default:
                await DisplayAlert("Info", "Raw output is only available for ZPL, EPL, and CPCL formats.", "OK");
                return;
        }
        
        var filePath = Path.Combine(FileSystem.Current.CacheDirectory, $"output.{fileExtension}");
        await File.WriteAllTextAsync(filePath, rawOutput);
        
        await DisplayAlert("Raw Output", $"Output saved to: {filePath}\n\nFirst 500 chars:\n{rawOutput.Substring(0, Math.Min(500, rawOutput.Length))}", "OK");
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private PrintOptions GetPrintOptions()
    {
        return new PrintOptions
        {
            PrinterId = _selectedPrinter?.Id ?? "",
            Copies = (int)CopiesStepper.Value,
            PaperSize = PaperSizePicker.SelectedItem?.ToString() ?? "4x6",
            Orientation = OrientationPicker.SelectedIndex == 0 ? PrintOrientation.Portrait : PrintOrientation.Landscape,
            Quality = (PrintQuality)QualityPicker.SelectedIndex,
            Darkness = (int)DarknessSlider.Value,
            PrintSpeed = (int)SpeedSlider.Value,
            CutAfterPrint = CutAfterCheckBox.IsChecked,
            PeelOff = PeelOffCheckBox.IsChecked,
            LabelWidthMm = int.TryParse(LabelWidthEntry.Text, out int w) ? w : 100,
            LabelHeightMm = int.TryParse(LabelHeightEntry.Text, out int h) ? h : 150,
            LabelPrinterLanguage = LanguagePicker.SelectedIndex switch
            {
                1 => "EPL",
                2 => "CPCL",
                _ => "ZPL"
            }
        };
    }
}
