using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Mimre.Api.RateLimiting;

public static class RateLimitingConfiguration
{
    private static string GetClientIp(HttpContext context) =>
    context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    public static IServiceCollection AddMimreRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Global rejection response
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var retryAfter = context.Lease.TryGetMetadata(
                    MetadataName.RetryAfter, out var retry)
                    ? (int)retry.TotalSeconds
                    : 60;

                context.HttpContext.Response.Headers.RetryAfter =
                    retryAfter.ToString();

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    title = "Too many requests.",
                    status = 429,
                    retryAfterSeconds = retryAfter
                }, ct);
            };

            // ── Auth — Fixed Window ───────────────────────────────────────────
            // Strict: 10 attempts per minute per IP to prevent brute force
            options.AddPolicy(RateLimitingPolicies.Auth, 
                context => RateLimitPartition.GetFixedWindowLimiter(partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown", factory: _ => new FixedWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = 10,
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }));

            // ── Upload — Token Bucket ─────────────────────────────────────────
            // Allows burst of 20 uploads, refills 2 tokens per second
            // Prevents storage abuse while allowing reasonable batch uploads
            options.AddPolicy(RateLimitingPolicies.Upload, 
                context => RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown", factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 20,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                        TokensPerPeriod = 2,
                        AutoReplenishment = true,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }));

            // ── General — Sliding Window ──────────────────────────────────────
            // 100 requests per minute, checked every 15 seconds
            // Smooth limiting for general API usage
            options.AddPolicy(RateLimitingPolicies.General, 
                context => RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown", factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }));

            // ── Client Gallery — Fixed Window ─────────────────────────────────
            // Generous: 200 requests per minute for public gallery viewing
            options.AddPolicy(RateLimitingPolicies.Client, 
                context => RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown", factory: _ => new FixedWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 200,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }));
        });

        return services;
    }
}
