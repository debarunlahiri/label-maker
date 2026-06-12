// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Text;
using Microsoft.Maui.Graphics;
using LabelMaker.Models;

namespace LabelMaker.Services;

public interface IPrintService
{
    Task PrintAsync(LabelTemplate template, PrintSettings settings);
    Task PrintPreviewAsync(LabelTemplate template);
    Task<string> GeneratePrintHtmlAsync(LabelTemplate template, PrintSettings settings);
    Task<byte[]> GeneratePrintPdfAsync(LabelTemplate template, PrintSettings settings);
}

public class PrintSettings
{
    public int Copies { get; set; } = 1;
    public string PaperSize { get; set; } = "A4";
    public string Orientation { get; set; } = "Portrait";
    public bool FitToPage { get; set; } = true;
    public bool ShowPrinterDialog { get; set; } = true;
    public double MarginTop { get; set; } = 10;
    public double MarginBottom { get; set; } = 10;
    public double MarginLeft { get; set; } = 10;
    public double MarginRight { get; set; } = 10;
}

public class PrintService : IPrintService
{
    public async Task PrintAsync(LabelTemplate template, PrintSettings settings)
    {
        var html = await GeneratePrintHtmlAsync(template, settings);
        var filePath = Path.Combine(FileSystem.Current.CacheDirectory, "print.html");
        await File.WriteAllTextAsync(filePath, html);
        await Launcher.OpenAsync(new OpenFileRequest("Print Label", new ReadOnlyFile(filePath)));
    }

    public async Task PrintPreviewAsync(LabelTemplate template)
    {
        var settings = new PrintSettings { ShowPrinterDialog = false };
        var html = await GeneratePrintHtmlAsync(template, settings);
        var filePath = Path.Combine(FileSystem.Current.CacheDirectory, "preview.html");
        await File.WriteAllTextAsync(filePath, html);
        await Launcher.OpenAsync(new OpenFileRequest("Print Preview", new ReadOnlyFile(filePath)));
    }

    public async Task<string> GeneratePrintHtmlAsync(LabelTemplate template, PrintSettings settings)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine($"    <title>{template.Name}</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        @page {");
        sb.AppendLine($"            size: {settings.PaperSize} {settings.Orientation.ToLower()};");
        sb.AppendLine($"            margin: {settings.MarginTop}mm {settings.MarginRight}mm {settings.MarginBottom}mm {settings.MarginLeft}mm;");
        sb.AppendLine("        }");
        sb.AppendLine("        body {");
        sb.AppendLine("            margin: 0;");
        sb.AppendLine("            padding: 0;");
        sb.AppendLine("            font-family: Arial, Helvetica, sans-serif;");
        sb.AppendLine("            -webkit-print-color-adjust: exact;");
        sb.AppendLine("            print-color-adjust: exact;");
        sb.AppendLine("        }");
        sb.AppendLine("        .label-container {");
        sb.AppendLine("            position: relative;");
        sb.AppendLine($"            width: {template.Width}px;");
        sb.AppendLine($"            height: {template.Height}px;");
        sb.AppendLine($"            background-color: {template.BackgroundColor.ToRgbHex()};");
        sb.AppendLine("            border: 1px solid #000;");
        sb.AppendLine("            box-sizing: border-box;");
        sb.AppendLine("            overflow: hidden;");
        sb.AppendLine("            page-break-after: always;");
        sb.AppendLine("        }");
        sb.AppendLine("        .element {");
        sb.AppendLine("            position: absolute;");
        sb.AppendLine("            box-sizing: border-box;");
        sb.AppendLine("            overflow: hidden;");
        sb.AppendLine("        }");
        sb.AppendLine("        .barcode-placeholder {");
        sb.AppendLine("            font-family: monospace;");
        sb.AppendLine("            font-size: 12px;");
        sb.AppendLine("            letter-spacing: 2px;");
        sb.AppendLine("            text-align: center;");
        sb.AppendLine("            background: repeating-linear-gradient(90deg, #000, #000 2px, #fff 2px, #fff 4px);");
        sb.AppendLine("            height: 60px;");
        sb.AppendLine("            display: flex;");
        sb.AppendLine("            align-items: flex-end;");
        sb.AppendLine("            justify-content: center;");
        sb.AppendLine("            padding-bottom: 4px;");
        sb.AppendLine("            color: #000;");
        sb.AppendLine("            font-weight: bold;");
        sb.AppendLine("        }");
        sb.AppendLine("        .qr-placeholder {");
        sb.AppendLine("            background: repeating-linear-gradient(0deg, #000, #000 4px, #fff 4px, #fff 8px);");
        sb.AppendLine("            display: flex;");
        sb.AppendLine("            align-items: center;");
        sb.AppendLine("            justify-content: center;");
        sb.AppendLine("            font-family: monospace;");
        sb.AppendLine("            font-size: 10px;");
        sb.AppendLine("            color: #000;");
        sb.AppendLine("        }");
        sb.AppendLine("        @media print {");
        sb.AppendLine("            body { padding: 0; margin: 0; }");
        sb.AppendLine("            .no-print { display: none; }");
        sb.AppendLine("        }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        
        // Print controls (visible only in browser, not in print)
        sb.AppendLine("    <div class=\"no-print\" style=\"padding: 10px; background: #f0f0f0; border-bottom: 1px solid #ccc;\">");
        sb.AppendLine("        <button onclick=\"window.print()\" style=\"padding: 8px 16px; font-size: 14px; cursor: pointer;\">Print</button>");
        sb.AppendLine("        <button onclick=\"window.close()\" style=\"padding: 8px 16px; font-size: 14px; cursor: pointer; margin-left: 8px;\">Close</button>");
        sb.AppendLine($"        <span style=\"margin-left: 20px;\">Label: <strong>{template.Name}</strong></span>");
        sb.AppendLine("    </div>");
        
        // Generate label copies
        for (int i = 0; i < settings.Copies; i++)
        {
            sb.AppendLine("    <div class=\"label-container\">");
            
            foreach (var element in template.Elements)
            {
                RenderElementHtml(sb, element);
            }
            
            sb.AppendLine("    </div>");
        }
        
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    public Task<byte[]> GeneratePrintPdfAsync(LabelTemplate template, PrintSettings settings)
    {
        // PDF generation would require a library like SkiaSharp or PDFSharp
        // For now, we return empty and rely on HTML printing
        return Task.FromResult(Array.Empty<byte>());
    }

    private void RenderElementHtml(StringBuilder sb, LabelElement element)
    {
        var styles = $"left:{element.X}px;top:{element.Y}px;width:{element.Width}px;height:{element.Height}px;";
        
        switch (element.Type)
        {
            case ElementType.Text:
                var textEl = (TextElement)element;
                var fontStyle = (textEl.Bold ? "font-weight:bold;" : "") + 
                              (textEl.Italic ? "font-style:italic;" : "") +
                              (textEl.Underline ? "text-decoration:underline;" : "");
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}{fontStyle}font-size:{textEl.FontSize}px;color:{textEl.TextColor.ToRgbHex()};font-family:{textEl.FontFamily},Arial,sans-serif;text-align:{textEl.HorizontalAlignment.ToString().ToLower()};display:flex;align-items:{(textEl.VerticalAlignment == TextAlignment.Start ? "flex-start" : textEl.VerticalAlignment == TextAlignment.End ? "flex-end" : "center")};padding:2px;\">");
                sb.AppendLine($"            {System.Net.WebUtility.HtmlEncode(textEl.Text)}");
                sb.AppendLine("        </div>");
                break;
                
            case ElementType.Rectangle:
                var rectEl = (ShapeElement)element;
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}background-color:{(rectEl.Filled ? rectEl.BackgroundColor.ToRgbHex() : "transparent")};border:{rectEl.BorderWidth}px solid {rectEl.BorderColor.ToRgbHex()};\">");
                sb.AppendLine("        </div>");
                break;
                
            case ElementType.Circle:
                var circleEl = (ShapeElement)element;
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}background-color:{(circleEl.Filled ? circleEl.BackgroundColor.ToRgbHex() : "transparent")};border:{circleEl.BorderWidth}px solid {circleEl.BorderColor.ToRgbHex()};border-radius:50%;\">");
                sb.AppendLine("        </div>");
                break;

            case ElementType.RoundedRectangle:
                var rrEl = (ShapeElement)element;
                var rrRadius = rrEl.CornerRadius > 0 ? rrEl.CornerRadius : 10;
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}background-color:{(rrEl.Filled ? rrEl.BackgroundColor.ToRgbHex() : "transparent")};border:{rrEl.BorderWidth}px solid {rrEl.BorderColor.ToRgbHex()};border-radius:{rrRadius}px;\">");
                sb.AppendLine("        </div>");
                break;

            case ElementType.Ellipse:
                var ellEl = (ShapeElement)element;
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}background-color:{(ellEl.Filled ? ellEl.BackgroundColor.ToRgbHex() : "transparent")};border:{ellEl.BorderWidth}px solid {ellEl.BorderColor.ToRgbHex()};border-radius:50%;\">");
                sb.AppendLine("        </div>");
                break;

            case ElementType.Triangle:
                var triEl = (ShapeElement)element;
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}background-color:{(triEl.Filled ? triEl.BackgroundColor.ToRgbHex() : "transparent")};border:{triEl.BorderWidth}px solid {triEl.BorderColor.ToRgbHex()};clip-path:polygon(50% 0%, 100% 100%, 0% 100%);\">");
                sb.AppendLine("        </div>");
                break;
                
            case ElementType.Line:
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}background-color:{element.BorderColor.ToRgbHex()};height:{element.BorderWidth}px;\">");
                sb.AppendLine("        </div>");
                break;
                
            case ElementType.Barcode:
                var barEl = (BarcodeElement)element;
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}display:flex;flex-direction:column;align-items:center;justify-content:center;background-color:white;border:1px solid #ccc;\">");
                sb.AppendLine("            <div class=\"barcode-placeholder\">");
                sb.AppendLine($"                {barEl.Data}");
                sb.AppendLine("            </div>");
                if (barEl.ShowText)
                {
                    sb.AppendLine($"            <div style=\"font-size:10px; margin-top:2px;\">{barEl.Data}</div>");
                }
                sb.AppendLine("        </div>");
                break;
                
            case ElementType.QRCode:
                var qrEl = (QRCodeElement)element;
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}display:flex;align-items:center;justify-content:center;background-color:white;border:1px solid #ccc;\">");
                sb.AppendLine("            <div class=\"qr-placeholder\">");
                sb.AppendLine($"                QR: {qrEl.Data}");
                sb.AppendLine("            </div>");
                sb.AppendLine("        </div>");
                break;
                
            case ElementType.Image:
                var imgEl = (ImageElement)element;
                if (!string.IsNullOrEmpty(imgEl.ImagePath) && File.Exists(imgEl.ImagePath))
                {
                    try
                    {
                        var imageBytes = File.ReadAllBytes(imgEl.ImagePath);
                        var base64 = Convert.ToBase64String(imageBytes);
                        var ext = Path.GetExtension(imgEl.ImagePath).ToLower();
                        var mimeType = ext == ".png" ? "image/png" : ext == ".jpg" || ext == ".jpeg" ? "image/jpeg" : "image/gif";
                        sb.AppendLine($"        <div class=\"element\" style=\"{styles}\">");
                        sb.AppendLine($"            <img src=\"data:{mimeType};base64,{base64}\" style=\"width:100%;height:100%;object-fit:contain;\" />");
                        sb.AppendLine("        </div>");
                    }
                    catch
                    {
                        sb.AppendLine($"        <div class=\"element\" style=\"{styles}background-color:#f0f0f0;display:flex;align-items:center;justify-content:center;font-size:10px;\">[Image]</div>");
                    }
                }
                else
                {
                    sb.AppendLine($"        <div class=\"element\" style=\"{styles}background-color:#f0f0f0;display:flex;align-items:center;justify-content:center;font-size:10px;\">[Image]</div>");
                }
                break;

            case ElementType.DateTime:
                var dtEl = (DateTimeElement)element;
                var dtDisplay = dtEl.ValueType switch
                {
                    DateTimeValueType.CurrentDate => DateTime.Now.ToString(dtEl.Format),
                    DateTimeValueType.CurrentTime => DateTime.Now.ToString("HH:mm:ss"),
                    DateTimeValueType.CurrentDateTime => DateTime.Now.ToString(dtEl.Format + " HH:mm:ss"),
                    DateTimeValueType.OffsetDate => DateTime.Now.AddDays(dtEl.OffsetDays).ToString(dtEl.Format),
                    _ => DateTime.Now.ToString(dtEl.Format)
                };
                var dtFontStyle = (dtEl.Bold ? "font-weight:bold;" : "");
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}{dtFontStyle}font-size:{dtEl.FontSize}px;color:{dtEl.TextColor.ToRgbHex()};font-family:{dtEl.FontFamily},Arial,sans-serif;text-align:{dtEl.HorizontalAlignment.ToString().ToLower()};display:flex;align-items:center;justify-content:center;\">");
                sb.AppendLine($"            {System.Net.WebUtility.HtmlEncode(dtDisplay)}");
                sb.AppendLine("        </div>");
                break;

            case ElementType.Counter:
                var cntEl = (CounterElement)element;
                var cntDisplay = $"{cntEl.Prefix}{cntEl.StartValue.ToString().PadLeft(cntEl.Padding, '0')}{cntEl.Suffix}";
                var cntFontStyle = (cntEl.Bold ? "font-weight:bold;" : "");
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}{cntFontStyle}font-size:{cntEl.FontSize}px;color:{cntEl.TextColor.ToRgbHex()};font-family:{cntEl.FontFamily},Arial,sans-serif;display:flex;align-items:center;justify-content:center;\">");
                sb.AppendLine($"            {System.Net.WebUtility.HtmlEncode(cntDisplay)}");
                sb.AppendLine("        </div>");
                break;

            case ElementType.DatabaseField:
                var dbEl = (DatabaseFieldElement)element;
                var dbFontStyle = (dbEl.Bold ? "font-weight:bold;" : "");
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}{dbFontStyle}font-size:{dbEl.FontSize}px;color:{dbEl.TextColor.ToRgbHex()};font-family:{dbEl.FontFamily},Arial,sans-serif;text-align:{dbEl.HorizontalAlignment.ToString().ToLower()};display:flex;align-items:center;justify-content:center;background-color:#E8F0FE;padding:2px;\">");
                sb.AppendLine($"            {{{System.Net.WebUtility.HtmlEncode(dbEl.FieldName)}}}");
                sb.AppendLine("        </div>");
                break;

            case ElementType.RFID:
                var rfidEl = (RFIDElement)element;
                var rfidFontStyle = (rfidEl.Bold ? "font-weight:bold;" : "");
                sb.AppendLine($"        <div class=\"element\" style=\"{styles}{rfidFontStyle}font-size:{rfidEl.FontSize}px;color:{rfidEl.TextColor.ToRgbHex()};font-family:{rfidEl.FontFamily},Arial,sans-serif;display:flex;align-items:center;justify-content:center;background-color:#FFF3E0;padding:2px;border:1px dashed #FF9800;\">");
                sb.AppendLine($"            RFID: {System.Net.WebUtility.HtmlEncode(rfidEl.EpcValue)}");
                sb.AppendLine("        </div>");
                break;
        }
    }
}
