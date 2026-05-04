using Microsoft.Extensions.Hosting.Internal;
using Mimre.Application;
using Mimre.Infrastructure;
using Mimre.Infrastructure.Queue;
using Mimre.Worker.Logging;
using Mimre.Worker.Workers;
using Serilog;

SerilogConfiguration.Configure(new HostingEnvironment { EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production" });


try
{
    Log.Information("Starting Mimre Worker...");

    var builder = Host.CreateApplicationBuilder(args);

    // ── Services ─────────────────────────────────────────────────────────────────

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Queue settings needed directly by the worker
    builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(nameof(QueueSettings)));

    // Register the background worker
    builder.Services.AddHostedService<PhotoProcessingWorker>();

    // ── Build & Run ───────────────────────────────────────────────────────────────

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Mimre Worker failed to start.");
}
finally
{
    Log.CloseAndFlush();
}

