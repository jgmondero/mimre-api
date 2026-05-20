using Microsoft.Extensions.Hosting.Internal;
using Mimre.Application;
using Mimre.Infrastructure;
using Mimre.Infrastructure.Queue;
using Mimre.Worker.Logging;
using Mimre.Worker.Workers;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var environment = new HostingEnvironment
{
    EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"
};

SerilogConfiguration.Configure(environment, configuration);


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

