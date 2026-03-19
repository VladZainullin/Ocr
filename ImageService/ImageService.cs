using ImageMagick;
using ImageService.Contracts;
using Microsoft.Extensions.Logging;

namespace ImageService;

internal sealed partial class ImageService(ILogger<ImageService> logger) : IImageService
{
    public byte[] Prepare(ReadOnlySpan<byte> bytes)
    {
        try
        {
            using var image = new MagickImage(bytes);
            
            image.AutoOrient();

            image.Alpha(AlphaOption.Remove);
            
            image.ColorSpace = ColorSpace.Gray;
            image.Depth = 8;
            
            image.Normalize();
            
            image.Strip();
            
            image.Format = MagickFormat.Tiff;

            return image.ToByteArray();
        }
        catch (Exception e)
        {
            LogImageError(logger, e);
            return [];
        }
    }
    
    public byte[] Prepare(Stream stream)
    {
        try
        {
            using var image = new MagickImage(stream);
            
            image.AutoOrient();

            image.Alpha(AlphaOption.Remove);
            
            image.ColorSpace = ColorSpace.Gray;
            image.Depth = 8;
            
            image.Normalize();
            
            image.Strip();
            
            image.Format = MagickFormat.Tiff;

            return image.ToByteArray();
        }
        catch (Exception e)
        {
            LogImageError(logger, e);
            return [];
        }
    }

    [LoggerMessage(LogLevel.Error, "Image error")]
    internal static partial void LogImageError(ILogger<ImageService> logger, Exception e);
}