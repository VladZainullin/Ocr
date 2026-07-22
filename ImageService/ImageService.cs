using System.Diagnostics;
using ImageMagick;
using ImageService.Contracts;
using Microsoft.Extensions.Logging;

namespace ImageService;

internal sealed partial class ImageService(ILogger<ImageService> logger, ActivitySource activitySource) : IImageService
{
    public bool TryPrepare(ReadOnlySpan<byte> bytes, out byte[] data, out uint width, out uint height, out uint bytesPerPixel)
    {
        data = [];
        width = 0;
        height = 0;
        bytesPerPixel = 0;
        
        using var activity = activitySource.StartActivity();
        if (bytes.IsEmpty) return false;
        
        try
        {
            using var image = new MagickImage(bytes);
            
            if (image.Width == 0 || image.Height == 0)
                return false;
            
            image.AutoOrient();
            
            image.Alpha(AlphaOption.Remove);
            image.BackgroundColor = MagickColors.White;
            
            image.ColorType = ColorType.Grayscale;
            image.Depth = 8;
            
            image.Deskew(new Percentage(10));
            
            image.MedianFilter(3);
            
            image.Threshold(new Percentage(50));
            
            image.ColorFuzz = new Percentage(10);
            image.Trim();
            image.ResetPage();
            
            image.Format = MagickFormat.Tiff;
            image.Settings.Compression = CompressionMethod.Group4;
            
            image.Strip();

            data = image.ToByteArray();
            height = image.Height;
            width = image.Width;
            bytesPerPixel = image.ChannelCount;
            
            return true;
        }
        catch (Exception e)
        {
            LogImageError(logger, e);
            return false;
        }
    }

    [LoggerMessage(LogLevel.Error, "Image error")]
    internal static partial void LogImageError(ILogger<ImageService> logger, Exception e);
}