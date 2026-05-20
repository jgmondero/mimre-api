using MediatR;
using Mimre.Api.RateLimiting;
using Mimre.Api.Services;
using Mimre.Application.DTOs;
using Mimre.Application.Features.Photos.Commands.DeletePhoto;
using Mimre.Application.Features.Photos.Commands.UploadPhoto;
using Mimre.Application.Features.Photos.Queries.GetPhotosByAlbum;

namespace Mimre.Api.Endpoints;

public static class PhotoEndpoints
{
    public static void MapPhotoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/photos")
            .WithTags("Photos")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitingPolicies.General);

        group.MapGet("/album/{albumId:guid}", async (Guid albumId, ISender sender, Guid? cursor = null, int pageSize = 30) =>
        {
            var result = await sender.Send(new GetPhotosByAlbumQuery(albumId, cursor, pageSize));
            return Results.Ok(result);
        });

        // Multipart file upload
        group.MapPost("/upload/{albumId:guid}", async (
            Guid albumId,
            IFormFileCollection files,
            ISender sender,
            CurrentUserService currentUser) =>
        {

            if (files.Count == 0)
                return Results.BadRequest("No files uploaded.");

            var results = new List<PhotoDto>();

            foreach (var file in files)
            {
                await using var stream = file.OpenReadStream();

                var command = new UploadPhotoCommand(
                    AlbumId: albumId,
                    UserId: currentUser.UserId,
                    FileName: file.FileName,
                    ContentType: file.ContentType,
                    FileSizeBytes: file.Length,
                    FileStream: stream);

                var result = await sender.Send(command);
                results.Add(result);
            }

            return Results.Ok(results);
        })
        .DisableAntiforgery() // Required for multipart in Minimal APIs
        .RequireRateLimiting(RateLimitingPolicies.Upload);

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeletePhotoCommand(id));
            return Results.NoContent();
        });
    }
}
