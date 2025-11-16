using ImageMagick;

namespace Web;

internal sealed class ImageService
{
    public byte[] Recognition(Memory<byte> bytes)
    {
        try
        {
            using var image = new MagickImage(bytes.Span);

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