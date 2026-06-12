using LabelForge.Core.Models.Printing;
using LabelForge.Core.Enums;

namespace LabelForge.PrintAgent.Services;

public class PrinterDiscoveryService
{
    private readonly ILogger<PrinterDiscoveryService> _logger;
    private readonly List<PrinterInfo> _discoveredPrinters = [];

    public PrinterDiscoveryService(ILogger<PrinterDiscoveryService> logger)
    {
        _logger = logger;
    }

    public async Task<List<PrinterInfo>> DiscoverPrintersAsync()
    {
        _logger.LogInformation("Discovering printers...");
        _discoveredPrinters.Clear();

        if (OperatingSystem.IsWindows())
        {
            await DiscoverWindowsPrintersAsync();
        }
        else if (OperatingSystem.IsMacOS())
        {
            await DiscoverMacPrintersAsync();
        }

        _logger.LogInformation("Discovered {Count} printers", _discoveredPrinters.Count);
        return _discoveredPrinters;
    }

    public List<PrinterInfo> GetDiscoveredPrinters() => _discoveredPrinters;

    private async Task DiscoverWindowsPrintersAsync()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Get-Printer | Select-Object Name, DriverName, PortName | ConvertTo-Json\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(psi);
            if (process != null)
            {
                var output = await process.StandardOutput.ReadToEndAsync();
                _logger.LogDebug("Windows printer discovery output: {Output}", output);
                // TODO: Parse JSON output and populate _discoveredPrinters
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to discover Windows printers via PowerShell");
        }
    }

    private async Task DiscoverMacPrintersAsync()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "lpstat",
                Arguments = "-p -d",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(psi);
            if (process != null)
            {
                var output = await process.StandardOutput.ReadToEndAsync();
                _logger.LogDebug("macOS printer discovery output: {Output}", output);
                // TODO: Parse lpstat output and populate _discoveredPrinters
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to discover macOS printers via lpstat");
        }
    }
}