using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Mimre.Api.Endpoints;
using Mimre.Api.Logging;
using Mimre.Api.Middleware;
using Mimre.Api.Services;
using Mimre.Application;
using Mimre.Infrastructure;
using Mimre.Infrastructure.Auth;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

SerilogConfiguration.Configure(new HostingEnvironment { EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production" }, configuration);

try
{
    Log.Information("Starting Mimre API...");

    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ──────────────────────────────────────────────────────────────
    builder.Host.UseSerilog();

    // ── Services ────────────────────────────────────────────────────────────────

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Current user
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<CurrentUserService>();

    // JWT Authentication
    var jwtSettings = builder.Configuration
        .GetSection(nameof(JwtSettings))
        .Get<JwtSettings>()!;

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

    // OpenAPI
    builder.Services.AddOpenApi();
    builder.Services.AddOpenApi(options =>
    {
        options.AddSchemaTransformer((schema, context, ct) =>
        {
            if (context.JsonTypeInfo.Type == typeof(Guid) ||
                context.JsonTypeInfo.Type == typeof(Guid?))
            {
                schema.Type = JsonSchemaType.String;
                schema.Format = "uuid";
            }
            return Task.CompletedTask;
        });
    });

    // Allow large file uploads (100 MB)
    builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB total
        options.ValueCountLimit = 1024; // max number of files
    });

    // ── Pipeline ─────────────────────────────────────────────────────────────────

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Mimre API";
            options.Theme = ScalarTheme.Purple;
        });
    }

    app.UseAuthentication();
    app.UseAuthorization();

    // ── Endpoints ────────────────────────────────────────────────────────────────

    app.MapAuthEndpoints();
    app.MapGalleryEndpoints();
    app.MapAlbumEndpoints();
    app.MapPhotoEndpoints();
    app.MapShareLinkEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Mimre API failed to start.");
}
finally
{
    Log.CloseAndFlush();
}


