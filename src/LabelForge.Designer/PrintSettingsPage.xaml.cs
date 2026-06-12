// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using LabelMaker.Models;
using LabelMaker.Services;

namespace LabelMaker;

public partial class PrintSettingsPage : ContentPage
{
    private readonly LabelTemplate _template;
    private readonly PrintService _printService;

    public PrintSettingsPage(LabelTemplate template)
    {
        InitializeComponent();
        _template = template;
        _printService = new PrintService();
    }

    private void OnCopiesChanged(object sender, ValueChangedEventArgs e)
    {
        CopiesLabel.Text = $"{(int)e.NewValue} copy{(e.NewValue > 1 ? "ies" : "y")}";
    }

    private async void OnPreviewClicked(object sender, EventArgs e)
    {
        var settings = GetSettings();
        await _printService.PrintPreviewAsync(_template);
    }

    private async void OnPrintClicked(object sender, EventArgs e)
    {
        var settings = GetSettings();
        await _printService.PrintAsync(_template, settings);
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private PrintSettings GetSettings()
    {
        return new PrintSettings
        {
            Copies = (int)CopiesStepper.Value,
            PaperSize = PaperSizePicker.SelectedItem?.ToString() ?? "A4",
            Orientation = OrientationPicker.SelectedItem?.ToString() ?? "Portrait",
            FitToPage = FitToPageCheckBox.IsChecked,
            MarginTop = double.TryParse(MarginTopEntry.Text, out double top) ? top : 10,
            MarginBottom = double.TryParse(MarginBottomEntry.Text, out double bottom) ? bottom : 10,
            MarginLeft = double.TryParse(MarginLeftEntry.Text, out double left) ? left : 10,
            MarginRight = double.TryParse(MarginRightEntry.Text, out double right) ? right : 10
        };
    }
}
