using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Albums.Commands.UpdateAlbum;

public class UpdateAlbumCommandHandler(IUnitOfWork uow) : IRequestHandler<UpdateAlbumCommand>
{
    public async Task Handle(UpdateAlbumCommand request, CancellationToken ct)
    {
        var album = await uow.Albums.GetByIdAsync(request.AlbumId, ct)
            ?? throw new NotFoundException(nameof(Album), request.AlbumId);

        album.Rename(request.Title);
        album.Reorder(request.SortOrder);
        await uow.SaveChangesAsync(ct);
    }
}
