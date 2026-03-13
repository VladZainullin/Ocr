using Domain;

namespace OcrService.Contracts;

public interface IOcrService
{
    IReadOnlyCollection<BlockModel> Recognition(byte[] bytes);
}
