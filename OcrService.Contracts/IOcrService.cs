using Domain;

namespace OcrService.Contracts;

public interface IOcrService
{
    List<BlockModel> Recognition(byte[] bytes);
}
