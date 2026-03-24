using System.Text;
using Domain;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using Tesseract;

namespace OcrService;

internal sealed class OcrService(
    ObjectPool<TesseractEngine> tesseractEngineObjectPool, 
    ObjectPool<StringBuilder> stringBuilderObjectPool) : IOcrService
{
    public IReadOnlyCollection<BlockModel> Recognition(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        
        var tesseractEngine = tesseractEngineObjectPool.Get();
        
        var blocks = new List<BlockModel>();
        
        var currentLineStringBuilder = stringBuilderObjectPool.Get();
        List<string> words = [];
        
        var currentBlockStringBuilder = stringBuilderObjectPool.Get();
        List<LineModel> textLines = [];
        
        try
        {
            using var pix = Pix.LoadFromMemory(bytes);
            using var page = tesseractEngine.Process(pix);
            using var iterator = page.GetIterator();

            iterator.Begin();
            
            do
            {
                if (iterator.IsAtBeginningOf(PageIteratorLevel.Block))
                {
                    textLines = [];
                }

                if (iterator.IsAtBeginningOf(PageIteratorLevel.TextLine))
                {
                    words = [];
                }

                var word = iterator.GetText(PageIteratorLevel.Word);
                if (!string.IsNullOrWhiteSpace(word))
                {
                    words.Add(word);
                        
                    if (currentLineStringBuilder.Length > 0)
                        currentLineStringBuilder.Append(' ');
                    currentLineStringBuilder.Append(word);

                    if (currentBlockStringBuilder.Length > 0)
                        currentBlockStringBuilder.Append(' ');
                    currentBlockStringBuilder.Append(word);
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word) && words.Count > 0)
                {
                    textLines.Add(new LineModel
                    {
                        Text = currentLineStringBuilder.ToString(),
                        Words = words
                    });
                    
                    currentLineStringBuilder.Clear();
                    words = [];
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Word) && textLines.Count > 0)
                {
                    blocks.Add(new BlockModel
                    {
                        Text = currentBlockStringBuilder.ToString(),
                        Lines = textLines
                    });
                    
                    currentBlockStringBuilder.Clear();
                    textLines = [];
                }
            } while (iterator.Next(PageIteratorLevel.Word));
        }
        finally
        {
            currentLineStringBuilder.Clear();
            currentBlockStringBuilder.Clear();
    
            stringBuilderObjectPool.Return(currentLineStringBuilder);
            stringBuilderObjectPool.Return(currentBlockStringBuilder);

            
            tesseractEngineObjectPool.Return(tesseractEngine);
        }

        return blocks;
    }
}
