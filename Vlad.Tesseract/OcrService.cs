using Domain.Builders;
using Domain.Models;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

internal sealed class OcrService(
    ObjectPool<TesseractEngine> tesseractEngineObjectPool,
    ObjectPool<ImageBuilder> imageBuilderObjectPool,
    ObjectPool<BlockBuilder> blockBuilderObjectPool,
    ObjectPool<LineBuilder> lineBuilderObjectPool) : IOcrService
{
    public ImageModel? Recognition(byte[] bytes)
    {
        var imageBuilder = imageBuilderObjectPool.Get();
        var blockBuilder = blockBuilderObjectPool.Get();
        var lineBuilder = lineBuilderObjectPool.Get();
        var tesseractEngine = tesseractEngineObjectPool.Get();

        using var pix = new Pix(bytes);
        using var monitor = new TesseractMonitor();
        monitor.SetCancel(words =>
        {
            Console.WriteLine($"Processed words: {words}");
            return false;
        });
        tesseractEngine.SetImage(pix);
        
        tesseractEngine.Recognize(monitor);

        _ = tesseractEngine.Text;

        var iterator = tesseractEngine.GetIterator();
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

                if (iterator.IsAtFinalElement(PageIteratorLevel.Line, PageIteratorLevel.Word))
                {
                    var lineModel = lineBuilder.Build();
                    if (lineModel != null)
                    {
                        blockBuilder.AddLine(lineModel);
                    }
                }
                
                if (iterator.IsAtFinalElement(PageIteratorLevel.Block, PageIteratorLevel.Word))
                {
                    var blockModel = blockBuilder.Build();
                    if (blockModel != null)
                    {
                        imageBuilder.AddBlock(blockModel);
                    }
                }
            } while (iterator.NextElement(PageIteratorLevel.Word));
            
            return imageBuilder.Build();
            
        }
        finally
        {
            iterator.Dispose();
            imageBuilderObjectPool.Return(imageBuilder);
            blockBuilderObjectPool.Return(blockBuilder);
            lineBuilderObjectPool.Return(lineBuilder);
            tesseractEngineObjectPool.Return(tesseractEngine);
        }
    }
}