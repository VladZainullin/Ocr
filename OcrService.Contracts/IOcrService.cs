using Domain;

namespace OcrService.Contracts;

public interface IOcrService
{
    ImageModel Recognition(byte[] bytes);
}
