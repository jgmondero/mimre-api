using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using Mimre.Domain.Interfaces.Services;
using System.Text.Json;

namespace Mimre.Infrastructure.Queue;

public class AzureStorageQueueService(IOptions<QueueSettings> options) : IStorageQueueService
{
    private readonly QueueClient _client = new(
        options.Value.ConnectionString,
        options.Value.PhotoProcessingQueue);

    public async Task EnqueuePhotoProcessingAsync(Guid photoId, CancellationToken ct = default)
    {
        await _client.CreateIfNotExistsAsync(cancellationToken: ct);

        var message = JsonSerializer.Serialize(new { PhotoId = photoId });
        // Azure Storage Queue requires Base64-encoded messages
        var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));
        await _client.SendMessageAsync(encoded, cancellationToken: ct);
    }
}
