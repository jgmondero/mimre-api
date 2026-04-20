namespace Mimre.Domain.Interfaces.Services;

public interface IStorageQueueService
{
    Task EnqueuePhotoProcessingAsync(Guid photoId, CancellationToken ct = default);
}
