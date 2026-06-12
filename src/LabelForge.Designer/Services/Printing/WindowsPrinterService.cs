// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace LabelMaker.Services.Printing;

public class WindowsPrinterService : IPrinterService
{
    public Task<List<PrinterInfo>> GetPrintersAsync()
    {
        var printers = new List<PrinterInfo>();
        
        try
        {
            // Use PowerShell to get printer list
            var psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Get-Printer | Select-Object Name, PortName, DriverName, Default, Type, Location, Comment, PrinterStatus | ConvertTo-Json -AsArray\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(psi);
            if (process == null) return Task.FromResult(printers);
            
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            if (!string.IsNullOrEmpty(output))
            {
                // Parse PowerShell output
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains("Name"))
                    {
                        var name = ExtractValue(line, "Name");
                        if (!string.IsNullOrEmpty(name))
                        {
                            printers.Add(new PrinterInfo
                            {
                                Id = name,
                                Name = name,
                                IsDefault = line.Contains("True"),
                                IsLabelPrinter = IsLabelPrinterName(name),
                                Type = IsLabelPrinterName(name) ? PrinterType.Label : PrinterType.Standard
                            });
                        }
                    }
                }
            }
        }
        catch
        {
            // Fallback to wmic if PowerShell fails
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "printer get Name,Default,PortName /format:csv",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                using var process = Process.Start(psi);
                if (process == null) return Task.FromResult(printers);
                
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines.Skip(1)) // Skip header
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        var name = parts[1].Trim();
                        if (!string.IsNullOrEmpty(name))
                        {
                            printers.Add(new PrinterInfo
                            {
                                Id = name,
                                Name = name,
                                IsDefault = line.Contains("TRUE"),
                                IsLabelPrinter = IsLabelPrinterName(name)
                            });
                        }
                    }
                }
            }
            catch { }
        }
        
        // Add common label printers that might be on network
        if (printers.Count == 0)
        {
            printers.Add(new PrinterInfo
            {
                Id = "Zebra_ZPL",
                Name = "Zebra ZPL (Raw TCP)",
                Port = "9100",
                IsLabelPrinter = true,
                Type = PrinterType.Label,
                LabelPrinterLanguage = "ZPL"
            });
            
            printers.Add(new PrinterInfo
            {
                Id = "Zebra_EPL",
                Name = "Zebra EPL (Raw TCP)",
                Port = "9100",
                IsLabelPrinter = true,
                Type = PrinterType.Label,
                LabelPrinterLanguage = "EPL"
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
                FileName = "powershell",
                Arguments = "-Command \"(Get-Printer -Default).Name\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(psi);
            if (process == null) return Task.FromResult<PrinterInfo?>(null);
            
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            
            if (!string.IsNullOrEmpty(output))
            {
                return Task.FromResult<PrinterInfo?>(new PrinterInfo
                {
                    Id = output,
                    Name = output,
                    IsDefault = true
                });
            }
        }
        catch { }
        
        return Task.FromResult<PrinterInfo?>(null);
    }

    public async Task<bool> PrintAsync(LabelMaker.Models.LabelTemplate template, PrintOptions options)
    {
        // Generate HTML and print via browser
        var htmlService = new PrintService();
        var html = await htmlService.GeneratePrintHtmlAsync(template, new PrintSettings());
        return await PrintHtmlAsync(html, options);
    }

    public Task<bool> PrintRawAsync(byte[] data, PrintOptions options)
    {
        // Send raw data to printer
        if (IsNetworkPrinter(options))
        {
            return SendRawToNetworkPrinter(data, options);
        }
        
        // Use Windows API or file
        return SendRawToWindowsPrinter(data, options);
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
            // Try to print via default browser
            var psi = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c start msedge \"{filePath}\"",
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
            // For network printers on port 9100
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

    private Task<bool> SendRawToWindowsPrinter(byte[] data, PrintOptions options)
    {
        try
        {
            // Write to file for testing
            var tempFile = Path.Combine(Path.GetTempPath(), $"raw_print_{Guid.NewGuid()}.bin");
            File.WriteAllBytes(tempFile, data);
            
            // Use lpr or copy to printer port
            return Task.FromResult(true);
        }
        catch
        {
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
        var labelKeywords = new[] { "zebra", "tsc", "godex", "cab", "sato", "datamax", "intermec", "brother", "dymo", "roland", "epson", "citizen", "toshiba", "printronix", "monarch", "paxar", "cab", "sato", " honeywell", " zebra" };
        return labelKeywords.Any(k => name.ToLower().Contains(k));
    }

    private string ExtractValue(string line, string key)
    {
        var match = Regex.Match(line, $"{key}\\s*:\\s*(.+)");
        return match.Success ? match.Groups[1].Value.Trim().TrimEnd(',') : "";
    }
}
