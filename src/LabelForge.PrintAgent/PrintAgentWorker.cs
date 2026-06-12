using LabelForge.PrintAgent.Services;

namespace LabelForge.PrintAgent;

public class PrintAgentWorker : BackgroundService
{
    private readonly PrintQueueService _printQueue;
    private readonly PrinterDiscoveryService _printerDiscovery;
    private readonly ILogger<PrintAgentWorker> _logger;
    private readonly IConfiguration _configuration;

    public PrintAgentWorker(
        PrintQueueService printQueue,
        PrinterDiscoveryService printerDiscovery,
        ILogger<PrintAgentWorker> logger,
        IConfiguration configuration)
    {
        _printQueue = printQueue;
        _printerDiscovery = printerDiscovery;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LabelForge Print Agent starting...");

        await _printerDiscovery.DiscoverPrintersAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await _printQueue.GetNextJobAsync();
                if (job != null)
                {
                    _logger.LogInformation("Processing print job {JobId}", job.Id);
                    await _printQueue.ProcessJobAsync(job);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing print job");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}