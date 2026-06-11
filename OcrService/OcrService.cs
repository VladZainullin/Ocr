using System.Diagnostics;
using System.Text;
using Domain;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using Tesseract;

namespace OcrService;

internal sealed class OcrService(
    ObjectPool<TesseractEngine> tesseractEngineObjectPool, 
    ObjectPool<StringBuilder> stringBuilderObjectPool,
    ActivitySource activitySource) : IOcrService
{
    public ImageModel Recognition(byte[] bytes)
    {
        using var activity = activitySource.StartActivity();
        ArgumentNullException.ThrowIfNull(bytes);
        
        var tesseractEngine = tesseractEngineObjectPool.Get();
        
        try
        {
            using var pix = Pix.LoadFromMemory(bytes);
            using var page = tesseractEngine.Process(pix);
            using var iterator = page.GetIterator();
            
            var blocks = ProcessIterator(iterator);

            return new ImageModel
            {
                Blocks = blocks,
            };
        }
        finally
        {
            tesseractEngineObjectPool.Return(tesseractEngine);
        }
    }

    private List<BlockModel> ProcessIterator(ResultIterator iterator)
    {
        var blocks = new List<BlockModel>();
        var currentBlock = new BlockBuilder(stringBuilderObjectPool);
        var currentLine = new LineBuilder(stringBuilderObjectPool);
        
        try
        {
            iterator.Begin();
            
            do
            {
                var word = GetWordText(iterator);
                
                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }
                
                currentLine.AddWord(word);
                currentBlock.AddWord(word);

                if (iterator.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                {
                    var lineModel = currentLine.Build();
                    if (lineModel != null)
                    {
                        currentBlock.AddLine(lineModel);
                    }
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Word))
                {
                    var blockModel = currentBlock.Build();
                    if (blockModel != null)
                    {
                        blocks.Add(blockModel);
                    }
                }
            } while (iterator.Next(PageIteratorLevel.Word));
            
            return blocks;
        }
        finally
        {
            currentBlock.Dispose();
            currentLine.Dispose();
        }
    }

    private static string GetWordText(ResultIterator iterator)
    {
        return iterator.GetText(PageIteratorLevel.Word);
    }
}
