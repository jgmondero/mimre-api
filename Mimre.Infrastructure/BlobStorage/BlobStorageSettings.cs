namespace Mimre.Infrastructure.BlobStorage;

public class BlobStorageSettings
{
    public string ConnectionString { get; init; } = default!;
    public string OriginalsContainer { get; init; } = "originals";
    public string ThumbnailsContainer { get; init; } = "thumbnails";
    public string WatermarkedContainer { get; init; } = "watermarked";
    public string ExportsContainer { get; init; } = "exports";
    public string CdnBaseUrl { get; init; } = default!;
}
