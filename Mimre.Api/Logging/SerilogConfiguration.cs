using Serilog;
using Serilog.Events;

namespace Mimre.Api.Logging;

public static class SerilogConfiguration
{
    public static void Configure(IHostEnvironment environment)
    {
        var logPath = Path.Combine("logs", "mimre-api-.log");

        var config = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command",
                environment.IsDevelopment()
                    ? LogEventLevel.Information  // show SQL in dev
                    : LogEventLevel.Warning)     // hide SQL in prod
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                "{Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
                                "{Message:lj} {Properties:j}{NewLine}{Exception}");

        // Write to Seq in development for structured log viewing
        if (environment.IsDevelopment())
        {
            config.WriteTo.Seq("http://localhost:5341");
        }

        Log.Logger = config.CreateLogger();
    }
}
