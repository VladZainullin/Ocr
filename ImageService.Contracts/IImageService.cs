namespace ImageService.Contracts;

public interface IImageService
{
    public bool TryPrepare(ReadOnlySpan<byte> bytes, out byte[] data, out uint width, out uint height,
        out uint bytesPerPixel);
}