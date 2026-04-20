using Mimre.Application;
using Mimre.Infrastructure;
using Mimre.Infrastructure.Queue;
using Mimre.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────────

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Queue settings needed directly by the worker
builder.Services.Configure<QueueSettings>(
    builder.Configuration.GetSection(nameof(QueueSettings)));

// Register the background worker
builder.Services.AddHostedService<PhotoProcessingWorker>();

// ── Build & Run ───────────────────────────────────────────────────────────────

var host = builder.Build();
host.Run();
