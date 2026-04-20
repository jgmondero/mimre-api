using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.ShareLinks.Commands.CreateShareLink;

public class CreateShareLinkCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateShareLinkCommand, ShareLinkDto>
{
    public async Task<ShareLinkDto> Handle(CreateShareLinkCommand request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetByIdAsync(request.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.GalleryId);

        if (gallery.UserId != request.UserId)
            throw new DomainException("Access denied.");

        var link = ShareLink.Create(
            request.GalleryId,
            request.ClientEmail,
            request.DownloadPermission,
            request.ExpiresAt);

        uow.ShareLinks.Add(link);
        await uow.SaveChangesAsync(ct);

        return new ShareLinkDto(
            link.Id,
            link.GalleryId,
            link.Token,
            link.ClientEmail,
            link.DownloadPermission,
            link.ExpiresAt,
            link.ViewCount,
            ShareUrl: $"https://mimre.app/c/{link.Token}");
    }
}
