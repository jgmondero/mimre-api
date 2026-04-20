using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Albums.Commands.CreateAlbum;

public class CreateAlbumCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateAlbumCommand, AlbumDto>
{
    public async Task<AlbumDto> Handle(CreateAlbumCommand request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetByIdAsync(request.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.GalleryId);

        var album = Album.Create(gallery.Id, request.Title, request.SortOrder);
        uow.Albums.Add(album);
        await uow.SaveChangesAsync(ct);

        return new AlbumDto(album.Id, album.GalleryId, album.Title, album.SortOrder, PhotoCount: 0);
    }
}
