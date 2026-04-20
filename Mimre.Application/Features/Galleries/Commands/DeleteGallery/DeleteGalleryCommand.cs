using MediatR;

namespace Mimre.Application.Features.Galleries.Commands.DeleteGallery;

public record DeleteGalleryCommand(
    Guid GalleryId, 
    Guid UserId) : IRequest;
