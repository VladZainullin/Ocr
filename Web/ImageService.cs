using ImageMagick;

namespace Web;

internal sealed class ImageService
{
    public byte[] Recognition(byte[] bytes)
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