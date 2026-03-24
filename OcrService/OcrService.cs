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
        var words = new List<string>();
        
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

                    words = [];
                    currentLineStringBuilder.Clear();
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Word) && textLines.Count > 0)
                {
                    blocks.Add(new BlockModel
                    {
                        Text = currentBlockStringBuilder.ToString(),
                        Lines = textLines
                    });

                    textLines = [];
                    currentBlockStringBuilder.Clear();
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
