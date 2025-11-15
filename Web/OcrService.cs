using Microsoft.Extensions.ObjectPool;
using Tesseract;

namespace Web;

internal sealed class OcrService(ObjectPool<TesseractEngine> pool)
{
    public IEnumerable<Block> Process(byte[] bytes)
    {
        var tesseractEngine = pool.Get();
        try
        {
            var pix = Pix.LoadFromMemory(bytes);
            using var page = tesseractEngine.Process(pix);
            using var iterator = page.GetIterator();
            
            Block? currentBlock = null;
            Line? currentLine = null;
            
            do
            {
                if (iterator.IsAtBeginningOf(PageIteratorLevel.Block))
                {
                    currentBlock = new Block();
                }

                if (iterator.IsAtBeginningOf(PageIteratorLevel.TextLine))
                {
                    currentLine = new Line();
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