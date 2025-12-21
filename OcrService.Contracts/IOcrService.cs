using Domain;

namespace OcrService.Contracts;

public interface IOcrService
{
    IEnumerable<BlockModel> Process(byte[] bytes);
}