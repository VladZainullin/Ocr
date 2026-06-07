namespace ImageService.Contracts;

public interface IImageService
{
    byte[] Prepare(ReadOnlySpan<byte> bytes);
}