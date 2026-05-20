using MediatR;
using Mimre.Api.RateLimiting;
using Mimre.Application.Features.Auth.Commands.Login;
using Mimre.Application.Features.Auth.Commands.Register;

namespace Mimre.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/auth")
            .WithTags("Auth")
            .RequireRateLimiting(RateLimitingPolicies.Auth);

        group.MapPost("/register", async (RegisterCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/users/{result.Id}", result);
        });

        group.MapPost("/login", async (LoginCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            // Set refresh token as HttpOnly cookie
            return Results.Ok(new
            {
                accessToken = result.AccessToken,
                user = result.User
            });
        });
    }
}
