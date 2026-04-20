namespace Mimre.Domain.Interfaces.Services;

public record ProcessedImage(
    Stream ThumbnailStream,
    Stream? WatermarkedStream,
    int Width,
    int Height);

public interface IImageProcessingService
{
    Task<ProcessedImage> ProcessAsync(Stream original, bool applyWatermark, CancellationToken ct = default);
}
