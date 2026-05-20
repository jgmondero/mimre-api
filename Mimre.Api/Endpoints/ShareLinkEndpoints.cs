using MediatR;
using Mimre.Api.RateLimiting;
using Mimre.Api.Services;
using Mimre.Application.Features.ShareLinks.Commands.CreateShareLink;
using Mimre.Application.Features.ShareLinks.Commands.DeleteShareLink;
using Mimre.Application.Features.ShareLinks.Queries.GetGalleryByToken;
using Mimre.Application.Features.ShareLinks.Queries.GetShareLinksByGallery;

namespace Mimre.Api.Endpoints;

public static class ShareLinkEndpoints
{
    public static void MapShareLinkEndpoints(this IEndpointRouteBuilder app)
    {
        // Authenticated routes — photographer manages share links
        var group = app
            .MapGroup("/api/share-links")
            .WithTags("ShareLinks")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitingPolicies.General);

        group.MapGet("/gallery/{galleryId:guid}", async (Guid galleryId, ISender sender, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new GetShareLinksByGalleryQuery(galleryId, page, pageSize));
            return Results.Ok(result);
        });

        group.MapPost("/", async (CreateShareLinkCommand command, ISender sender, CurrentUserService currentUser) =>
        {
            var result = await sender.Send(command with { UserId = currentUser.UserId });
            return Results.Created($"/api/share-links/{result.Id}", result);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteShareLinkCommand(id));
            return Results.NoContent();
        });

        // Public client-facing route — no auth required, token-gated
        app.MapGet("/api/client/{token}", async (string token, ISender sender) =>
        {
            var result = await sender.Send(new GetGalleryByTokenQuery(token));
            return Results.Ok(result);
        })
        .WithTags("Client")
        .RequireRateLimiting(RateLimitingPolicies.Client);
    }
}
