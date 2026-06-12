using LabelForge.Core.Models.Templates;
using LabelForge.Core.Models.Printing;
using LabelForge.Core.Enums;
using Xunit;

namespace LabelForge.Core.Tests;

public class LabelTemplateTests
{
    [Fact]
    public void LabelTemplate_DefaultValues_AreCorrect()
    {
        var template = new LabelTemplate();

        Assert.NotEqual(Guid.Empty, template.Id);
        Assert.Equal("Untitled", template.Name);
        Assert.Equal(400, template.Width);
        Assert.Equal(300, template.Height);
        Assert.Equal("#FFFFFF", template.BackgroundColor);
        Assert.Equal(203, template.Dpi);
        Assert.Equal(UnitType.Pixels, template.Unit);
        Assert.Equal(TemplateStatus.Draft, template.Status);
        Assert.Empty(template.Elements);
    }

    [Fact]
    public void TextElement_DefaultValues_AreCorrect()
    {
        var element = new TextElement();

        Assert.Equal(ElementType.Text, element.Type);
        Assert.Equal("Text", element.Text);
        Assert.Equal("Arial", element.FontFamily);
        Assert.Equal(12, element.FontSize);
        Assert.Equal(150, element.Width);
        Assert.Equal(30, element.Height);
    }

    [Fact]
    public void BarcodeElement_DefaultValues_AreCorrect()
    {
        var element = new BarcodeElement();

        Assert.Equal(ElementType.Barcode, element.Type);
        Assert.Equal(BarcodeType.Code128, element.BarcodeType);
        Assert.True(element.ShowHumanReadableText);
        Assert.Equal(200, element.Width);
        Assert.Equal(100, element.Height);
    }

    [Fact]
    public void PrintJob_DefaultValues_AreCorrect()
    {
        var job = new PrintJob();

        Assert.NotEqual(Guid.Empty, job.Id);
        Assert.Equal(PrintJobStatus.Created, job.Status);
        Assert.Equal(1, job.Copies);
        Assert.Empty(job.Logs);
    }

    [Fact]
    public void PrinterInfo_DefaultValues_AreCorrect()
    {
        var printer = new PrinterInfo();

        Assert.NotEqual(Guid.Empty, printer.Id);
        Assert.Equal(PrinterStatus.Unknown, printer.Status);
        Assert.Equal(ConnectionType.USB, printer.ConnectionType);
        Assert.True(printer.IsActive);
        Assert.Equal(203, printer.Dpi);
    }
}