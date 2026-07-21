using MediatR;
using Mimre.Api.Services;
using Mimre.Application.Features.Users.Commands.UpdateProfile;
using Mimre.Application.Features.Users.Queries.GetCurrentUser;

namespace Mimre.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization()
            .RequireRateLimiting("general");

        group.MapGet("/me", async (
            ISender sender,
            CurrentUserService currentUser) =>
        {
            var result = await sender.Send(new GetCurrentUserQuery(currentUser.UserId));
            return Results.Ok(result);
        })
        .WithName("GetCurrentUser")
        .WithSummary("Get the current authenticated user's profile");

        group.MapPut("/me", async (
            UpdateProfileCommand command,
            ISender sender,
            CurrentUserService currentUser) =>
        {
            await sender.Send(command with { UserId = currentUser.UserId });
            return Results.NoContent();
        })
        .WithName("UpdateProfile")
        .WithSummary("Update the current user's profile");
    }
}
