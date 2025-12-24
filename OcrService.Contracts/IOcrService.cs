using Domain;

namespace OcrService.Contracts;

public interface IOcrService
{
    List<BlockModel> Process(byte[] bytes);
}