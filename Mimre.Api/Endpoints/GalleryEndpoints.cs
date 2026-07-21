using MediatR;
using Mimre.Api.RateLimiting;
using Mimre.Api.Services;
using Mimre.Application.Features.Galleries.Commands.CreateGallery;
using Mimre.Application.Features.Galleries.Commands.DeleteGallery;
using Mimre.Application.Features.Galleries.Commands.PublishGallery;
using Mimre.Application.Features.Galleries.Commands.SetGalleryPassword;
using Mimre.Application.Features.Galleries.Commands.UpdateGallery;
using Mimre.Application.Features.Galleries.Queries.GetGalleries;
using Mimre.Application.Features.Galleries.Queries.GetGalleryById;
using Mimre.Application.Features.Galleries.Queries.GetGalleryBySlug;

namespace Mimre.Api.Endpoints;

public static class GalleryEndpoints
{
    public static void MapGalleryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/galleries")
            .WithTags("Galleries")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitingPolicies.General);

        group.MapGet("/", async (ISender sender, CurrentUserService currentUser, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new GetGalleriesQuery(currentUser.UserId, page, pageSize));
            return Results.Ok(result);
        });

        group.MapGet("/{slug}", async (string slug, ISender sender, CurrentUserService currentUser) =>
        {
            var result = await sender.Send(new GetGalleryBySlugQuery(currentUser.UserId, slug));
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CurrentUserService currentUser) =>
        {
            var result = await sender.Send(new GetGalleryByIdQuery(id, currentUser.UserId));
            return Results.Ok(result);
        })
        .WithName("GetGalleryById")
        .WithSummary("Get a gallery by ID");

        group.MapPost("/", async (CreateGalleryCommand command, ISender sender, CurrentUserService currentUser) =>
        {
            var result = await sender.Send(command with { UserId = currentUser.UserId });
            return Results.Created($"/api/galleries/{result.Slug}", result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateGalleryCommand command, ISender sender, CurrentUserService currentUser) =>
        {
            await sender.Send(command with { GalleryId = id, UserId = currentUser.UserId });
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CurrentUserService currentUser) =>
        {
            await sender.Send(new DeleteGalleryCommand(id, currentUser.UserId));
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/publish", async (Guid id, ISender sender, CurrentUserService currentUser) =>
        {
            await sender.Send(new PublishGalleryCommand(id, currentUser.UserId, Publish: true));
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/unpublish", async (Guid id, ISender sender, CurrentUserService currentUser) =>
        {
            await sender.Send(new PublishGalleryCommand(id, currentUser.UserId, Publish: false));
            return Results.NoContent();
        });

        group.MapPatch("/{id:guid}/password", async (Guid id, SetGalleryPasswordCommand command, ISender sender, CurrentUserService currentUser) =>
        {
            await sender.Send(command with { GalleryId = id, UserId = currentUser.UserId });
            return Results.NoContent();
        })
        .WithName("SetGalleryPassword")
        .WithSummary("Set or remove a gallery password");
    }
}
