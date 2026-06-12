using LabelForge.PrintAgent.Services;

namespace LabelForge.PrintAgent;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        if (OperatingSystem.IsWindows())
        {
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "LabelForge Print Agent";
            });
        }

        builder.Services.AddSingleton<PrinterDiscoveryService>();
        builder.Services.AddSingleton<PrintQueueService>();
        builder.Services.AddHostedService<PrintAgentWorker>();
        builder.Services.AddHostedService<PrinterStatusWorker>();

        builder.Services.AddHttpClient("LabelForgeServer", client =>
        {
            client.BaseAddress = new Uri(
                builder.Configuration["ServerUrl"] ?? "http://localhost:5000");
        });

        var host = builder.Build();
        await host.RunAsync();
    }
}