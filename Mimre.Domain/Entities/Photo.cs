using Mimre.Domain.Common;
using Mimre.Domain.Enums;
using Mimre.Domain.Events;

namespace Mimre.Domain.Entities;

public class Photo : AuditableEntity
{
    public Guid AlbumId { get; private set; }
    public string OriginalFileName { get; private set; } = default!;
    public string BlobPath { get; private set; } = default!;
    public string? ThumbnailBlobPath { get; private set; }
    public string? WatermarkedBlobPath { get; private set; }
    public long FileSizeBytes { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int SortOrder { get; private set; }
    public PhotoStatus Status { get; private set; } = PhotoStatus.Pending;
    public string? MetadataJson { get; private set; } // EXIF stored as JSON

    public Album Album { get; private set; } = default!;

    private Photo() { }

    public static Photo Create(
        Guid albumId,
        string originalFileName,
        string blobPath,
        long fileSizeBytes) =>
        new()
        {
            AlbumId = albumId,
            OriginalFileName = originalFileName,
            BlobPath = blobPath,
            FileSizeBytes = fileSizeBytes,
            Status = PhotoStatus.Pending
        };

    public void MarkProcessing() => Status = PhotoStatus.Processing;

    public void MarkReady(
        string thumbnailBlobPath,
        string? watermarkedBlobPath,
        int width,
        int height)
    {
        ThumbnailBlobPath = thumbnailBlobPath;
        WatermarkedBlobPath = watermarkedBlobPath;
        Width = width;
        Height = height;
        Status = PhotoStatus.Ready;
        RaiseDomainEvent(new PhotoUploadedEvent(Id, AlbumId));
    }

    public void MarkFailed() => Status = PhotoStatus.Failed;

    public void SetMetadata(string metadataJson) => MetadataJson = metadataJson;
    public void Reorder(int sortOrder) => SortOrder = sortOrder;
}
