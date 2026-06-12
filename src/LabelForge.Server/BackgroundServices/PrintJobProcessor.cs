using LabelForge.Core.Enums;
using LabelForge.Database;
using LabelForge.Core.Models.Printing;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Server.BackgroundServices;

public class PrintJobProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PrintJobProcessor> _logger;

    public PrintJobProcessor(IServiceProvider serviceProvider, ILogger<PrintJobProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Print Job Processor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LabelForgeDbContext>();

                var pendingJobs = await context.PrintJobs
                    .Where(j => j.Status == PrintJobStatus.Queued || j.Status == PrintJobStatus.Retrying)
                    .OrderBy(j => j.CreatedAt)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var job in pendingJobs)
                {
                    _logger.LogInformation("Processing print job {JobId} for template {TemplateId}", job.Id, job.TemplateId);
                    job.Status = PrintJobStatus.Processing;
                    job.StartedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync(stoppingToken);

                    context.PrintJobLogs.Add(new PrintJobLog
                    {
                        PrintJobId = job.Id,
                        Action = "Processing",
                        Message = "Print job processing started"
                    });
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Print Job Processor");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}