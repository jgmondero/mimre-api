using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;

namespace Mimre.Application.Features.Photos.Commands.UploadPhoto;

public class UploadPhotoCommandHandler(
    IUnitOfWork uow,
    IBlobStorageService blob,
    IStorageQueueService queue)
    : IRequestHandler<UploadPhotoCommand, PhotoDto>
{
    public async Task<PhotoDto> Handle(UploadPhotoCommand request, CancellationToken ct)
    {
        var album = await uow.Albums.GetByIdAsync(request.AlbumId, ct)
            ?? throw new NotFoundException(nameof(Album), request.AlbumId);

        var user = await uow.Users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        // Copy to MemoryStream to avoid stream disposal issues with multiple files
        using var memoryStream = new MemoryStream();
        await request.FileStream.CopyToAsync(memoryStream, ct);
        memoryStream.Position = 0;

        // Build blob path: originals/{userId}/{albumId}/{photoId}.ext
        var photoId = Guid.NewGuid();
        var ext = Path.GetExtension(request.FileName);
        var blobPath = $"originals/{request.UserId}/{request.AlbumId}/{photoId}{ext}";

        // Upload to blob storage
        await blob.UploadAsync(request.FileStream, blobPath, request.ContentType, ct);

        // Update user storage quota
        user.AddStorageUsage(request.FileSizeBytes);

        // Persist Photo record
        var photo = Photo.Create(request.AlbumId, request.FileName, blobPath, request.FileSizeBytes);
        uow.Photos.Add(photo);
        await uow.SaveChangesAsync(ct);

        // Enqueue for background processing (thumbnail + watermark)
        await queue.EnqueuePhotoProcessingAsync(photo.Id, ct);

        return new PhotoDto(photo.Id, photo.AlbumId, photo.OriginalFileName,
            ThumbnailUrl: string.Empty, WatermarkedUrl: null,
            photo.FileSizeBytes, photo.Width, photo.Height,
            photo.SortOrder, photo.Status);
    }
}
