using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Mimre.Api.Endpoints;
using Mimre.Api.Logging;
using Mimre.Api.Middleware;
using Mimre.Api.RateLimiting;
using Mimre.Api.Services;
using Mimre.Application;
using Mimre.Application.Common.Settings;
using Mimre.Infrastructure;
using Mimre.Infrastructure.Persistence;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    builder.Services.AddMimreRateLimiting();

    // OpenAPI
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, ct) =>
        {
            document.Info = new()
            {
                Title = "Mimre API",
                Version = "v1",
                Description = """
                Mimre is an online photo gallery platform for photographers.
                
                ## Authentication
                Most endpoints require a Bearer token obtained from `POST /api/auth/login`.
                Include it in the `Authorization` header: `Bearer {token}`
                
                ## Rate Limiting
                - Auth endpoints: 10 requests/minute per IP
                - Upload endpoints: 20 burst, 2/second refill per IP
                - General endpoints: 100 requests/minute per IP
                - Client endpoints: 200 requests/minute per IP
                """,
                Contact = new()
                {
                    Name = "Mimre Support",
                    Email = "monderojm.dev@gmail.com"
                }
            };

            document.Components ??= new();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token obtained from /api/auth/login"
            };

            return Task.CompletedTask;
        });

        options.AddSchemaTransformer((schema, context, ct) =>
        {
            var type = context.JsonTypeInfo.Type;
            if (type == typeof(Guid) || type == typeof(Guid?))
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

    using (var scope = app.Services.CreateScope())
    {
        var retries = 0;
        const int maxRetries = 10;
        while (retries < maxRetries)
        {
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<MimreDbContext>();
                await db.Database.MigrateAsync();
                break;
            }
            catch (Exception ex)
            {
                retries++;
                Log.Warning("Database not ready, retrying {Retry}/{Max}. Error: {Error}",
                    retries, maxRetries, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Mimre API";
            options.Theme = ScalarTheme.Purple;
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.Authentication = new ScalarAuthenticationOptions
            {
                PreferredSecuritySchemes = ["Bearer"]
            };
        });
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();

    // ── Endpoints ────────────────────────────────────────────────────────────────

    app.MapAuthEndpoints();
    app.MapGalleryEndpoints();
    app.MapAlbumEndpoints();
    app.MapPhotoEndpoints();
    app.MapShareLinkEndpoints();
    app.MapUserEndpoints();

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


