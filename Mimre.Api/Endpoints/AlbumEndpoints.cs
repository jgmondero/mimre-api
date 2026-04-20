using MediatR;
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
            .RequireAuthorization();

        group.MapGet("/gallery/{galleryId:guid}", async (Guid galleryId, ISender sender) =>
        {
            var result = await sender.Send(new GetAlbumsByGalleryQuery(galleryId));
            return Results.Ok(result);
        });

        group.MapPost("/", async (CreateAlbumCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/albums/{result.Id}", result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateAlbumCommand command, ISender sender) =>
        {
            await sender.Send(command with { AlbumId = id });
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteAlbumCommand(id));
            return Results.NoContent();
        });
    }
}
