using ImageMagick;
using ImageService.Contracts;
using Microsoft.Extensions.Logging;

namespace ImageService;

internal sealed class ImageService(ILogger<ImageService> logger) : IImageService
{
    public byte[] Recognition(ReadOnlySpan<byte> bytes)
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
            
            image.Format = MagickFormat.Png;

            return image.ToByteArray();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Image error");
            return [];
        }
    }
}