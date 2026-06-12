using LabelForge.Database;
using LabelForge.Core.Enums;
using LabelForge.Core.Models.Automation;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Server.BackgroundServices;

public class FileDropWatcher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FileDropWatcher> _logger;
    private readonly List<FileSystemWatcher> _watchers = [];

    public FileDropWatcher(IServiceProvider serviceProvider, ILogger<FileDropWatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("File Drop Watcher started.");

        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LabelForgeDbContext>();

                var fileDropIntegrations = await context.Integrations
                    .Where(i => i.TriggerType == TriggerType.FileDrop && i.IsActive)
                    .Include(i => i.Triggers)
                    .ToListAsync(stoppingToken);

                foreach (var integration in fileDropIntegrations)
                {
                    _logger.LogDebug("Checking file drop integration: {Name}", integration.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in File Drop Watcher");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var watcher in _watchers)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        _watchers.Clear();
        await base.StopAsync(cancellationToken);
    }
}