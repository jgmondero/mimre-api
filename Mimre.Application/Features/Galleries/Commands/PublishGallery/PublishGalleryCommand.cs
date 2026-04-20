using MediatR;

namespace Mimre.Application.Features.Galleries.Commands.PublishGallery;

public record PublishGalleryCommand(
    Guid GalleryId, 
    Guid UserId, 
    bool Publish) : IRequest;
