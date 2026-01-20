namespace ImageService.Contracts;

public interface IImageService
{
    byte[] Recognition(ReadOnlySpan<byte> bytes);
    
    byte[] Recognition(Stream stream);
}