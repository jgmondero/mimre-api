using MediatR;
using Mimre.Api.RateLimiting;
using Mimre.Api.Services;
using Mimre.Application.Features.Albums.Commands.CreateAlbum;
using Mimre.Application.Features.Albums.Commands.DeleteAlbum;
using Mimre.Application.Features.Albums.Commands.UpdateAlbum;
using Mimre.Application.Features.Albums.Queries.GetAlbumsByGallery;

namespace Mimre.Api.Endpoints;

public static class AlbumEndpoints
{
    public static void MapAlbumEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/albums")
            .WithTags("Albums")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitingPolicies.General);

        group.MapGet("/gallery/{galleryId:guid}", async (Guid galleryId, ISender sender, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new GetAlbumsByGalleryQuery(galleryId, page, pageSize));
            return Results.Ok(result);
        })
        .WithName("GetAlbumsByGallery")
        .WithSummary("Get all albums for a gallery");

        group.MapPost("/", async (CreateAlbumCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/albums/{result.Id}", result);
        })
        .WithName("CreateAlbum")
        .WithSummary("Create a new album in a gallery");

        group.MapPut("/{id:guid}", async (Guid id, UpdateAlbumCommand command, ISender sender, CurrentUserService currentUser) =>
        {
            await sender.Send(command with { AlbumId = id, UserId = currentUser.UserId });
            return Results.NoContent();
        })
        .WithName("UpdateAlbum")
        .WithSummary("Update an album title and sort order");

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CurrentUserService currentUser) =>
        {
            await sender.Send(new DeleteAlbumCommand(id, currentUser.UserId));
            return Results.NoContent();
        })
        .WithName("DeleteAlbum")
        .WithSummary("Delete an album and all its photos");
    }
}
