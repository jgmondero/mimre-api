using Mimre.Domain.Interfaces.Services;
using SkiaSharp;

namespace Mimre.Infrastructure.ImageProcessing;

public class SkiaImageProcessingService : IImageProcessingService
{
    private const int ThumbnailMaxDimension = 800;
    private const int JpegQuality = 85;

    public Task<ProcessedImage> ProcessAsync(
        Stream original,
        bool applyWatermark,
        CancellationToken ct = default)
    {
        using var inputStream = new SKManagedStream(original);
        using var sourceBitmap = SKBitmap.Decode(inputStream);

        var (thumbWidth, thumbHeight) = CalculateDimensions(
            sourceBitmap.Width, sourceBitmap.Height, ThumbnailMaxDimension);
        var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

        // Generate thumbnail as WebP
        using var thumbBitmap = sourceBitmap.Resize(new SKImageInfo(thumbWidth, thumbHeight), samplingOptions);
        using var thumbImage = SKImage.FromBitmap(thumbBitmap);

        var thumbStream = new MemoryStream();
        thumbImage.Encode(SKEncodedImageFormat.Webp, JpegQuality).SaveTo(thumbStream);
        thumbStream.Position = 0;

        // Generate watermarked version if requested
        Stream? watermarkedStream = null;
        if (applyWatermark)
        {
            using var wmBitmap = sourceBitmap.Copy();
            ApplyWatermark(wmBitmap);
            using var wmImage = SKImage.FromBitmap(wmBitmap);

            watermarkedStream = new MemoryStream();
            wmImage.Encode(SKEncodedImageFormat.Jpeg, JpegQuality).SaveTo(watermarkedStream);
            watermarkedStream.Position = 0;
        }

        return Task.FromResult(new ProcessedImage(
            thumbStream, watermarkedStream,
            sourceBitmap.Width, sourceBitmap.Height));
    }

    private static void ApplyWatermark(SKBitmap bitmap)
    {
        using var canvas = new SKCanvas(bitmap);
        using var font = new SKFont(SKTypeface.Default, Math.Max(bitmap.Width / 20f, 24f));
        using var paint = new SKPaint()
        {
            Color = SKColors.White.WithAlpha(100),
            IsAntialias = true,
        };

        const string watermarkText = "© Mimre";
        var textWidth = font.MeasureText(watermarkText);

        // Bottom-right corner with padding
        var x = bitmap.Width - textWidth - 20;
        var y = bitmap.Height - 20;

        canvas.DrawText(watermarkText, x, y, SKTextAlign.Right, font, paint);
    }

    private static (int width, int height) CalculateDimensions(int srcWidth, int srcHeight, int maxDimension)
    {
        if (srcWidth <= maxDimension && srcHeight <= maxDimension)
            return (srcWidth, srcHeight);

        var ratio = (double)srcWidth / srcHeight;
        return srcWidth > srcHeight
            ? (maxDimension, (int)(maxDimension / ratio))
            : ((int)(maxDimension * ratio), maxDimension);
    }
}
