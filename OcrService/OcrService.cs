using Domain;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using Tesseract;

namespace OcrService;

internal sealed class OcrService(ObjectPool<TesseractEngine> pool) : IOcrService
{
    public List<BlockModel> Process(byte[] bytes)
    {
        var tesseractEngine = pool.Get();
        var blocks = new List<BlockModel>();
        try
        {
            using var pix = Pix.LoadFromMemory(bytes);
            using var page = tesseractEngine.Process(pix);
            using var iterator = page.GetIterator();
            
            BlockModel? currentBlock = null;
            LineModel? currentLine = null;
            
            do
            {
                if (iterator.IsAtBeginningOf(PageIteratorLevel.Block))
                {
                    currentBlock = new BlockModel();
                }

                if (iterator.IsAtBeginningOf(PageIteratorLevel.TextLine))
                {
                    currentLine = new LineModel();
                }

                if (iterator.IsAtBeginningOf(PageIteratorLevel.Word))
                {
                    var word = iterator.GetText(PageIteratorLevel.Word);
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        currentLine?.Words.Add(word);
                    }
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word) && currentLine?.Words.Count > 0)
                {
                    currentBlock?.Lines.Add(currentLine);
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Word) && currentBlock?.Lines.Count > 0)
                {
                    blocks.Add(currentBlock);
                }
            } while (iterator.Next(PageIteratorLevel.Word));
        }
        finally
        {
            pool.Return(tesseractEngine);
        }

        return blocks;
    }
}