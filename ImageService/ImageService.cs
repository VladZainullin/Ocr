using ImageMagick;
using ImageService.Contracts;

namespace ImageService;

internal sealed class ImageService : IImageService
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
        catch (MagickMissingDelegateErrorException)
        {
            return [];
        }
    }
}