namespace ImageService.Contracts;

public interface IImageService
{
    public bool TryPrepare(ReadOnlySpan<byte> bytes, out byte[] data);
}