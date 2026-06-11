using System.Diagnostics;
using System.Text;
using Domain;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using Tesseract;
using Domain.Builders;

namespace OcrService;

internal sealed class OcrService(
    ObjectPool<TesseractEngine> tesseractEngineObjectPool, 
    ObjectPool<StringBuilder> stringBuilderObjectPool,
    ActivitySource activitySource) : IOcrService
{
    public ImageModel? Recognition(byte[] bytes)
    {
        using var activity = activitySource.StartActivity();
        ArgumentNullException.ThrowIfNull(bytes);
        
        var tesseractEngine = tesseractEngineObjectPool.Get();
        
        try
        {
            using var pix = Pix.LoadFromMemory(bytes);
            using var page = tesseractEngine.Process(pix);
            using var iterator = page.GetIterator();
            
            var imageModel = ProcessIterator(iterator);

            return imageModel;
        }
        finally
        {
            tesseractEngineObjectPool.Return(tesseractEngine);
        }
    }

    private ImageModel? ProcessIterator(ResultIterator iterator)
    {
        var imageBuilder = new ImageBuilder(stringBuilderObjectPool);
        var blockBuilder = new BlockBuilder(stringBuilderObjectPool);
        var lineBuilder = new LineBuilder(stringBuilderObjectPool);
        
        try
        {
            iterator.Begin();
            
            do
            {
                var word = iterator.GetText(PageIteratorLevel.Word);
                
                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }
                
                lineBuilder.AddWord(word);

                if (iterator.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                {
                    var lineModel = lineBuilder.Build();
                    if (lineModel != null)
                    {
                        blockBuilder.AddLine(lineModel);
                    }
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Word))
                {
                    var blockModel = blockBuilder.Build();
                    if (blockModel != null)
                    {
                        imageBuilder.AddBlock(blockModel);
                    }
                }
            } while (iterator.Next(PageIteratorLevel.Word));
            
            return imageBuilder.Build();
        }
        finally
        {
            imageBuilder.Dispose();
            blockBuilder.Dispose();
            lineBuilder.Dispose();
        }
    }
}
