using LabelForge.Database;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Server.BackgroundServices;

public class PrinterStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PrinterStatusUpdater> _logger;

    public PrinterStatusUpdater(IServiceProvider serviceProvider, ILogger<PrinterStatusUpdater> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Printer Status Updater started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LabelForgeDbContext>();

                var activePrinters = await context.Printers
                    .Where(p => p.IsActive)
                    .ToListAsync(stoppingToken);

                foreach (var printer in activePrinters)
                {
                    var interval = printer.Status == Core.Enums.PrinterStatus.Online
                        ? TimeSpan.FromSeconds(30)
                        : TimeSpan.FromMinutes(5);

                    if (printer.LastSeen == null || DateTime.UtcNow - printer.LastSeen > interval)
                    {
                        printer.LastSeen = DateTime.UtcNow;
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating printer status");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}