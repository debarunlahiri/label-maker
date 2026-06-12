using LabelForge.PrintAgent.Services;

namespace LabelForge.PrintAgent;

public class PrinterStatusWorker : BackgroundService
{
    private readonly PrinterDiscoveryService _printerDiscovery;
    private readonly ILogger<PrinterStatusWorker> _logger;

    public PrinterStatusWorker(PrinterDiscoveryService printerDiscovery, ILogger<PrinterStatusWorker> logger)
    {
        _printerDiscovery = printerDiscovery;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Printer Status Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _printerDiscovery.DiscoverPrintersAsync();
                // TODO: Push printer status updates to the central server via API
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating printer status");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}