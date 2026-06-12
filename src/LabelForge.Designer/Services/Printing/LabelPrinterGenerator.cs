// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Globalization;
using System.Text;
using LabelMaker.Models;
using LabelMaker.Services.Printing;

namespace LabelMaker.Services;

public class LabelPrinterGenerator : ILabelPrinterGenerator
{
    public string GenerateZpl(LabelTemplate template, PrintOptions options)
    {
        var sb = new StringBuilder();
        
        // ZPL header
        sb.AppendLine("^XA");
        sb.AppendLine("^CI28"); // UTF-8 encoding
        
        // Set darkness
        sb.AppendLine($"~SD{options.Darkness}");
        
        // Set print speed
        sb.AppendLine($"^PR{options.PrintSpeed}");
        
        // Label dimensions in dots (203 DPI = 8 dots per mm)
        int dotsPerMm = 8;
        int widthDots = options.LabelWidthMm * dotsPerMm;
        int heightDots = options.LabelHeightMm * dotsPerMm;
        
        sb.AppendLine($"^PW{widthDots}");
        sb.AppendLine($"^LL{heightDots}");
        
        foreach (var element in template.Elements)
        {
            RenderElementZpl(sb, element, dotsPerMm);
        }
        
        // End label
        if (options.CutAfterPrint)
            sb.AppendLine("^MMC"); // Cut after each label
        if (options.PeelOff)
            sb.AppendLine("^MMP"); // Peel off mode
        
        sb.AppendLine("^XZ");
        
        return sb.ToString();
    }

    public string GenerateEpl(LabelTemplate template, PrintOptions options)
    {
        var sb = new StringBuilder();
        
        // EPL2 header
        int dotsPerMm = 8;
        int widthDots = options.LabelWidthMm * dotsPerMm;
        int heightDots = options.LabelHeightMm * dotsPerMm;
        
        sb.AppendLine("N"); // Clear image buffer
        sb.AppendLine($"q{widthDots}"); // Label width
        sb.AppendLine($"Q{heightDots},24"); // Label height and gap
        
        foreach (var element in template.Elements)
        {
            RenderElementEpl(sb, element, dotsPerMm);
        }
        
        // Print command
        sb.AppendLine($"P{options.Copies},1"); // Print copies
        
        return sb.ToString();
    }

    public string GenerateCpcl(LabelTemplate template, PrintOptions options)
    {
        var sb = new StringBuilder();
        
        // CPCL header
        int dotsPerMm = 8;
        int widthDots = options.LabelWidthMm * dotsPerMm;
        int heightDots = options.LabelHeightMm * dotsPerMm;
        
        sb.AppendLine($"! 0 200 200 {heightDots} 1");
        sb.AppendLine($"PAGE-WIDTH {widthDots}");
        sb.AppendLine("SETBOLD 0");
        sb.AppendLine("SETMAG 1 1");
        
        foreach (var element in template.Elements)
        {
            RenderElementCpcl(sb, element, dotsPerMm);
        }
        
        sb.AppendLine("PRINT");
        
        return sb.ToString();
    }

    private void RenderElementZpl(StringBuilder sb, LabelElement element, int dotsPerMm)
    {
        int x = (int)(element.X * dotsPerMm / 3.78); // Convert to dots
        int y = (int)(element.Y * dotsPerMm / 3.78);
        int w = (int)(element.Width * dotsPerMm / 3.78);
        int h = (int)(element.Height * dotsPerMm / 3.78);
        
        switch (element.Type)
        {
            case ElementType.Text:
                var textEl = (TextElement)element;
                int fontSize = (int)(textEl.FontSize * 3); // Scale for ZPL
                string font = textEl.Bold ? "0" : "A";
                string alignment = textEl.HorizontalAlignment == TextAlignment.Start ? "L" : 
                                  textEl.HorizontalAlignment == TextAlignment.Center ? "C" : "R";
                
                sb.AppendLine($"^FO{x},{y}^A{font}N,{fontSize},{fontSize}^FB{w},1,0,{alignment}^FD{textEl.Text}^FS");
                break;
                
            case ElementType.Rectangle:
                var rectEl = (ShapeElement)element;
                int borderWidth = (int)(element.BorderWidth);
                
                if (rectEl.Filled)
                {
                    sb.AppendLine($"^FO{x},{y}^GB{w},{h},{h}^FS");
                }
                else
                {
                    sb.AppendLine($"^FO{x},{y}^GB{w},{h},{borderWidth}^FS");
                }
                break;
                
            case ElementType.Line:
                sb.AppendLine($"^FO{x},{y}^GB{w},{h},{(int)element.BorderWidth}^FS");
                break;
                
            case ElementType.Barcode:
                var barEl = (BarcodeElement)element;
                int barHeight = h - 20;
                
                sb.AppendLine($"^FO{x},{y}^BY2^BCN,{barHeight},Y,N,N^FD{barEl.Data}^FS");
                
                if (barEl.ShowText)
                {
                    sb.AppendLine($"^FO{x},{y + barHeight + 2}^A0N,12,12^FD{barEl.Data}^FS");
                }
                break;
                
            case ElementType.QRCode:
                var qrEl = (QRCodeElement)element;
                int qrSize = Math.Min(w, h);
                int qrMagnification = Math.Max(1, qrSize / 25);
                
                sb.AppendLine($"^FO{x},{y}^BQN,2,{qrMagnification}^FDQA,{qrEl.Data}^FS");
                break;
                
            case ElementType.Image:
                var imgEl = (ImageElement)element;
                if (!string.IsNullOrEmpty(imgEl.ImagePath) && File.Exists(imgEl.ImagePath))
                {
                    // For ZPL, images need to be converted to GRF format
                    // This is a simplified version - in production you'd convert the image
                    sb.AppendLine($"^FO{x},{y}^XGR:LOGO.GRF,1,1^FS");
                }
                break;

            case ElementType.RoundedRectangle:
            case ElementType.Ellipse:
            case ElementType.Circle:
                sb.AppendLine(rectEl != null ? $"^FO{x},{y}^GB{w},{h},{(int)rectEl.BorderWidth}^FS" : $"^FO{x},{y}^GB{w},{h},0^FS");
                break;

            case ElementType.Triangle:
                sb.AppendLine($"^FO{x},{y}^GB{w},{h},{(int)element.BorderWidth}^FS");
                break;

            case ElementType.DateTime:
                var dtEl = (DateTimeElement)element;
                var dtText = dtEl.ValueType switch
                {
                    DateTimeValueType.CurrentDate => DateTime.Now.ToString(dtEl.Format),
                    DateTimeValueType.CurrentTime => DateTime.Now.ToString("HH:mm:ss"),
                    DateTimeValueType.CurrentDateTime => DateTime.Now.ToString(dtEl.Format + " HH:mm:ss"),
                    DateTimeValueType.OffsetDate => DateTime.Now.AddDays(dtEl.OffsetDays).ToString(dtEl.Format),
                    _ => DateTime.Now.ToString(dtEl.Format)
                };
                var dtFontSize = (int)(dtEl.FontSize * 3);
                sb.AppendLine($"^FO{x},{y}^A{(dtEl.Bold ? "0" : "A")}N,{dtFontSize},{dtFontSize}^FD{dtText}^FS");
                break;

            case ElementType.Counter:
                var cntEl = (CounterElement)element;
                var cntText = $"{cntEl.Prefix}{cntEl.StartValue.ToString().PadLeft(cntEl.Padding, '0')}{cntEl.Suffix}";
                var cntFontSize = (int)(cntEl.FontSize * 3);
                sb.AppendLine($"^FO{x},{y}^A{(cntEl.Bold ? "0" : "A")}N,{cntFontSize},{cntFontSize}^FD{cntText}^FS");
                break;

            case ElementType.DatabaseField:
                var dbEl = (DatabaseFieldElement)element;
                var dbFontSize = (int)(dbEl.FontSize * 3);
                sb.AppendLine($"^FO{x},{y}^A{(dbEl.Bold ? "0" : "A")}N,{dbFontSize},{dbFontSize}^FD{{{dbEl.FieldName}}}^FS");
                break;

            case ElementType.RFID:
                var rfidEl = (RFIDElement)element;
                sb.AppendLine($"^FO{x},{y}^A0N,{(int)(rfidEl.FontSize * 3)},{(int)(rfidEl.FontSize * 3)}^FDRFID:{rfidEl.EpcValue}^FS");
                sb.AppendLine($"^RFW,E,{rfidEl.MemoryBank},{rfidEl.EpcValue.Length},{rfidEl.EpcValue}^FS");
                break;
        }
    }

    private void RenderElementEpl(StringBuilder sb, LabelElement element, int dotsPerMm)
    {
        int x = (int)(element.X * dotsPerMm / 3.78);
        int y = (int)(element.Y * dotsPerMm / 3.78);
        int w = (int)(element.Width * dotsPerMm / 3.78);
        int h = (int)(element.Height * dotsPerMm / 3.78);
        
        switch (element.Type)
        {
            case ElementType.Text:
                var textEl = (TextElement)element;
                int fontSize = (int)(textEl.FontSize / 3);
                fontSize = Math.Max(1, Math.Min(5, fontSize));
                
                sb.AppendLine($"A{fontSize},{x},{y},1,1,1,N,\"{textEl.Text}\"");
                break;
                
            case ElementType.Rectangle:
                var rectEl = (ShapeElement)element;
                if (rectEl.Filled)
                {
                    sb.AppendLine($"LO{x},{y},{w},{h}");
                }
                else
                {
                    int bw = (int)(element.BorderWidth);
                    sb.AppendLine($"X{x},{y},{w},{h},{bw}");
                }
                break;
                
            case ElementType.Line:
                sb.AppendLine($"LO{x},{y},{w},{h}");
                break;
                
            case ElementType.Barcode:
                var barEl = (BarcodeElement)element;
                sb.AppendLine($"B{x},{y},0,1C,2,{h - 20},N,N,\"{barEl.Data}\"");
                if (barEl.ShowText)
                {
                    sb.AppendLine($"A1,{x},{y + h - 15},1,1,1,N,\"{barEl.Data}\"");
                }
                break;
                
            case ElementType.QRCode:
                var qrEl = (QRCodeElement)element;
                int qrSize = Math.Min(w, h);
                sb.AppendLine($"b{x},{y},Q,{qrSize},2,QA,\"{qrEl.Data}\"");
                break;
                
            case ElementType.Image:
                var imgEl = (ImageElement)element;
                if (!string.IsNullOrEmpty(imgEl.ImagePath) && File.Exists(imgEl.ImagePath))
                {
                    sb.AppendLine($"G{x},{y},{w},{h},\"{imgEl.ImagePath}\"");
                }
                break;
        }
    }

    private void RenderElementCpcl(StringBuilder sb, LabelElement element, int dotsPerMm)
    {
        int x = (int)(element.X * dotsPerMm / 3.78);
        int y = (int)(element.Y * dotsPerMm / 3.78);
        int w = (int)(element.Width * dotsPerMm / 3.78);
        int h = (int)(element.Height * dotsPerMm / 3.78);
        
        switch (element.Type)
        {
            case ElementType.Text:
                var textEl = (TextElement)element;
                int fontSize = (int)(textEl.FontSize * 2);
                
                sb.AppendLine($"SETMAG 1 1");
                sb.AppendLine($"T 0 {fontSize} {x} {y} {textEl.Text}");
                break;
                
            case ElementType.Rectangle:
                var rectEl = (ShapeElement)element;
                if (rectEl.Filled)
                {
                    sb.AppendLine($"BOX {x} {y} {x + w} {y + h} 1 1");
                }
                else
                {
                    sb.AppendLine($"BOX {x} {y} {x + w} {y + h} {(int)element.BorderWidth} 1");
                }
                break;
                
            case ElementType.Line:
                sb.AppendLine($"LINE {x} {y} {x + w} {y + h} {(int)element.BorderWidth}");
                break;
                
            case ElementType.Barcode:
                var barEl = (BarcodeElement)element;
                sb.AppendLine($"BARCODE 128 {x} {y} {h} 0.50 {w} {h - 20} {barEl.Data}");
                if (barEl.ShowText)
                {
                    sb.AppendLine($"T 0 12 {x} {y + h - 15} {barEl.Data}");
                }
                break;
                
            case ElementType.QRCode:
                var qrEl = (QRCodeElement)element;
                int qrSize = Math.Min(w, h);
                sb.AppendLine($"BARCODE QR {x} {y} M 2 U {qrSize}");
                sb.AppendLine($"MA,{qrEl.Data}");
                sb.AppendLine($"ENDQR");
                break;
                
            case ElementType.Image:
                var imgEl = (ImageElement)element;
                if (!string.IsNullOrEmpty(imgEl.ImagePath) && File.Exists(imgEl.ImagePath))
                {
                    sb.AppendLine($"CG {x} {y} {w} {h} {imgEl.ImagePath}");
                }
                break;
        }
    }
}
