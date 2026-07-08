using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.ShareLinks.Commands.DeleteShareLink;

public class DeleteShareLinkCommandHandler(IUnitOfWork uow, ILogger<DeleteShareLinkCommandHandler> logger) : IRequestHandler<DeleteShareLinkCommand>
{
    public async Task Handle(DeleteShareLinkCommand request, CancellationToken ct)
    {
        var link = await uow.ShareLinks.GetByIdAsync(request.ShareLinkId, ct)
            ?? throw new NotFoundException(nameof(ShareLink), request.ShareLinkId);

        var gallery = await uow.Galleries.GetByIdAsync(link.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), link.GalleryId);

        if (gallery?.UserId != request.UserId)
            throw new UnauthorizedAccessException("Access denied.");

        uow.ShareLinks.Remove(link);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Share link deleted. {ShareLinkId}", request.ShareLinkId);
    }
}
