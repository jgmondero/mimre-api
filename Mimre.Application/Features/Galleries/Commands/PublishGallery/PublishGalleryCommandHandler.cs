using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Galleries.Commands.PublishGallery;

public class PublishGalleryCommandHandler(IUnitOfWork uow) : IRequestHandler<PublishGalleryCommand>
{
    public async Task Handle(PublishGalleryCommand request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetByIdAsync(request.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.GalleryId);

        if (gallery.UserId != request.UserId)
            throw new DomainException("Access denied.");

        if (request.Publish) gallery.Publish();
        else gallery.Unpublish();

        await uow.SaveChangesAsync(ct);
    }
}
