// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Diagnostics;
using System.Text;

namespace LabelMaker.Services.Printing;

public class MacPrinterService : IPrinterService
{
    public Task<List<PrinterInfo>> GetPrintersAsync()
    {
        var printers = new List<PrinterInfo>();
        
        try
        {
            // Use lpstat to get printers
            var psi = new ProcessStartInfo
            {
                FileName = "lpstat",
                Arguments = "-a",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(psi);
            if (process == null) return Task.FromResult(printers);
            
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    var name = parts[0].Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        printers.Add(new PrinterInfo
                        {
                            Id = name,
                            Name = name,
                            IsLabelPrinter = IsLabelPrinterName(name),
                            Type = IsLabelPrinterName(name) ? PrinterType.Label : PrinterType.Standard
                        });
                    }
                }
            }
            
            // Get default printer
            var defaultPsi = new ProcessStartInfo
            {
                FileName = "lpstat",
                Arguments = "-d",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var defaultProcess = Process.Start(defaultPsi);
            if (defaultProcess != null)
            {
                string defaultOutput = defaultProcess.StandardOutput.ReadToEnd();
                defaultProcess.WaitForExit();
                
                var defaultMatch = System.Text.RegularExpressions.Regex.Match(defaultOutput, @"system default destination: (.+)");
                if (defaultMatch.Success)
                {
                    var defaultName = defaultMatch.Groups[1].Value.Trim();
                    var defaultPrinter = printers.FirstOrDefault(p => p.Name == defaultName);
                    if (defaultPrinter != null)
                    {
                        defaultPrinter.IsDefault = true;
                    }
                }
            }
        }
        catch
        {
            // Fallback: add common printers
        }
        
        // Add label printer options if no printers found
        if (printers.Count == 0)
        {
            printers.Add(new PrinterInfo
            {
                Id = "Zebra_TCP",
                Name = "Zebra ZPL (Raw TCP)",
                Port = "9100",
                IsLabelPrinter = true,
                Type = PrinterType.Label,
                LabelPrinterLanguage = "ZPL"
            });
            
            printers.Add(new PrinterInfo
            {
                Id = "Epson_Label",
                Name = "Epson Label Printer",
                IsLabelPrinter = true,
                Type = PrinterType.Label
            });
        }
        
        return Task.FromResult(printers);
    }

    public Task<PrinterInfo?> GetDefaultPrinterAsync()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "lpstat",
                Arguments = "-d",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(psi);
            if (process == null) return Task.FromResult<PrinterInfo?>(null);
            
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            var match = System.Text.RegularExpressions.Regex.Match(output, @"system default destination: (.+)");
            if (match.Success)
            {
                var name = match.Groups[1].Value.Trim();
                return Task.FromResult<PrinterInfo?>(new PrinterInfo
                {
                    Id = name,
                    Name = name,
                    IsDefault = true
                });
            }
        }
        catch { }
        
        return Task.FromResult<PrinterInfo?>(null);
    }

    public async Task<bool> PrintAsync(LabelMaker.Models.LabelTemplate template, PrintOptions options)
    {
        var htmlService = new PrintService();
        var html = await htmlService.GeneratePrintHtmlAsync(template, new PrintSettings());
        return await PrintHtmlAsync(html, options);
    }

    public Task<bool> PrintRawAsync(byte[] data, PrintOptions options)
    {
        try
        {
            if (IsNetworkPrinter(options))
            {
                return SendRawToNetworkPrinter(data, options);
            }
            
            // Use lpr command
            var tempFile = Path.Combine(Path.GetTempPath(), $"raw_print_{Guid.NewGuid()}.bin");
            File.WriteAllBytes(tempFile, data);
            
            var psi = new ProcessStartInfo
            {
                FileName = "lpr",
                Arguments = $"-P \"{options.PrinterId}\" \"{tempFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(psi);
            if (process == null) return Task.FromResult(false);
            
            process.WaitForExit();
            return Task.FromResult(process.ExitCode == 0);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Print error: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public Task<bool> PrintZplAsync(string zplData, PrintOptions options)
    {
        return PrintRawAsync(Encoding.UTF8.GetBytes(zplData), options);
    }

    public Task<bool> PrintEplAsync(string eplData, PrintOptions options)
    {
        return PrintRawAsync(Encoding.UTF8.GetBytes(eplData), options);
    }

    public async Task<bool> PrintHtmlAsync(string html, PrintOptions options)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"print_{Guid.NewGuid()}.html");
        await File.WriteAllTextAsync(filePath, html);
        
        try
        {
            // Open in default browser
            var psi = new ProcessStartInfo
            {
                FileName = "open",
                Arguments = $"\"{filePath}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            
            Process.Start(psi);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<List<PrintJob>> GetPrintJobsAsync()
    {
        return Task.FromResult(new List<PrintJob>());
    }

    public Task<bool> CancelJobAsync(Guid jobId)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CheckPrinterStatusAsync(string printerId)
    {
        return Task.FromResult(true);
    }

    private Task<bool> SendRawToNetworkPrinter(byte[] data, PrintOptions options)
    {
        try
        {
            using var client = new System.Net.Sockets.TcpClient();
            var host = options.PrinterId.Contains(":") ? options.PrinterId.Split(':')[0] : options.PrinterId;
            var port = 9100;
            
            if (options.PrinterId.Contains(":"))
            {
                int.TryParse(options.PrinterId.Split(':')[1], out port);
            }
            
            client.Connect(host, port);
            using var stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Network print error: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    private bool IsNetworkPrinter(PrintOptions options)
    {
        return !string.IsNullOrEmpty(options.PrinterId) && 
               (options.PrinterId.Contains(":") || options.PrinterId.Contains("TCP"));
    }

    private bool IsLabelPrinterName(string name)
    {
        var labelKeywords = new[] { "zebra", "tsc", "godex", "cab", "sato", "datamax", "intermec", "brother", "dymo", "roland", "epson", "citizen", "toshiba", "printronix", "monarch", "paxar", " honeywell" };
        return labelKeywords.Any(k => name.ToLower().Contains(k));
    }
}
