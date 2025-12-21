using Domain;
using Microsoft.Extensions.ObjectPool;
using Tesseract;

namespace Ocr;

public sealed class OcrService(ObjectPool<TesseractEngine> pool)
{
    public IEnumerable<BlockModel> Process(byte[] bytes)
    {
        var tesseractEngine = pool.Get();
        try
        {
            var pix = Pix.LoadFromMemory(bytes);
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
                    yield return currentBlock;
                }
            } while (iterator.Next(PageIteratorLevel.Word));
        }
        finally
        {
            pool.Return(tesseractEngine);
        }
    }
}