namespace Mimre.Infrastructure.Queue;

public class QueueSettings
{
    public string ConnectionString { get; init; } = default!;
    public string PhotoProcessingQueue { get; init; } = "photo-processing";
}
