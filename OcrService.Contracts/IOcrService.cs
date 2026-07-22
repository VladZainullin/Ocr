using Domain;
using Domain.Models;

namespace OcrService.Contracts;

public interface IOcrService
{
    ImageModel? Recognition(byte[] bytes, uint width, uint height, uint bytesPerPixel);
}
