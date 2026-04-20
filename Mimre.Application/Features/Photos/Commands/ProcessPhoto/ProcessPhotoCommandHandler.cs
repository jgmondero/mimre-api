using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;

namespace Mimre.Application.Features.Photos.Commands.ProcessPhoto;

public class ProcessPhotoCommandHandler(
    IUnitOfWork uow,
    IBlobStorageService blob,
    IImageProcessingService imageProcessor)
    : IRequestHandler<ProcessPhotoCommand>
{
    public async Task Handle(ProcessPhotoCommand request, CancellationToken ct)
    {
        var photo = await uow.Photos.GetByIdAsync(request.PhotoId, ct)
            ?? throw new NotFoundException(nameof(Photo), request.PhotoId);

        photo.MarkProcessing();
        await uow.SaveChangesAsync(ct);

        try
        {
            using var originalStream = await blob.DownloadAsync(photo.BlobPath, ct);
            using var memoryStream = new MemoryStream();
            await originalStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;
            var applyWatermark = true; // could come from gallery settings in future
            var processed = await imageProcessor.ProcessAsync(memoryStream, applyWatermark, ct);

            var thumbPath = photo.BlobPath.Replace("originals/", "thumbnails/").Replace(
                Path.GetExtension(photo.BlobPath), "_thumb.webp");
            var wmPath = photo.BlobPath.Replace("originals/", "watermarked/");

            await blob.UploadAsync(processed.ThumbnailStream, thumbPath, "image/webp", ct);

            string? wmBlobPath = null;
            if (processed.WatermarkedStream is not null)
            {
                await blob.UploadAsync(processed.WatermarkedStream, wmPath, "image/jpeg", ct);
                wmBlobPath = wmPath;
            }

            photo.MarkReady(thumbPath, wmBlobPath, processed.Width, processed.Height);
        }
        catch
        {
            photo.MarkFailed();
        }
        finally
        {
            await uow.SaveChangesAsync(ct);
        }
    }
}
