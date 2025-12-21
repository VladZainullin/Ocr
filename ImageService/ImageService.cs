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

            image.Grayscale();

            image.MedianFilter(1);

            return image.ToByteArray();
        }
        catch (MagickMissingDelegateErrorException e)
        {
            logger.LogError(e, "Magick missing delegate error");
            return [];
        }
    }
}