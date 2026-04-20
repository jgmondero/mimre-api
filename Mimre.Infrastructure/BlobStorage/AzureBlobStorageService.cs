using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Mimre.Domain.Interfaces.Services;

namespace Mimre.Infrastructure.BlobStorage;

public class AzureBlobStorageService(IOptions<BlobStorageSettings> options) : IBlobStorageService
{
    private readonly BlobStorageSettings _settings = options.Value;
    private readonly BlobServiceClient _client = new(options.Value.ConnectionString);

    public async Task<string> UploadAsync(Stream stream, string blobPath, string contentType, CancellationToken ct = default)
    {
        var containerName = GetContainerFromPath(blobPath);
        var containerClient = _client.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = containerClient.GetBlobClient(blobPath);

        stream.Position = 0;
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string blobPath, CancellationToken ct = default)
    {
        var containerName = GetContainerFromPath(blobPath);
        var blobClient = _client
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobPath);

        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }

    public Task<string> GenerateSasUrlAsync(string blobPath, TimeSpan validity)
    {
        var containerName = GetContainerFromPath(blobPath);
        var blobClient = _client
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobPath);

        var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(validity));
        return Task.FromResult(sasUri.ToString());
    }

    public async Task<Stream> DownloadAsync(string blobPath, CancellationToken ct = default)
    {
        var containerName = GetContainerFromPath(blobPath);
        var blobClient = _client
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobPath);

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
        return response.Value.Content;
    }

    // Derives container name from the first segment of the blob path
    // e.g. "originals/{userId}/..." → "originals"
    private static string GetContainerFromPath(string blobPath) =>
        blobPath.Split('/')[0];
}
