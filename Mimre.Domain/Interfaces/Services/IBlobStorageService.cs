namespace Mimre.Domain.Interfaces.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string blobPath, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string blobPath, CancellationToken ct = default);
    Task<string> GenerateSasUrlAsync(string blobPath, TimeSpan validity);
    Task<Stream> DownloadAsync(string blobPath, CancellationToken ct = default);
}
