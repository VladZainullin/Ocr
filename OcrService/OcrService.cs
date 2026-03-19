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
        LineModel? currentLine = null;
        
        var currentBlockStringBuilder = stringBuilderObjectPool.Get();
        BlockModel? currentBlock = null;
        
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
                    currentBlock = new BlockModel();
                }

                if (iterator.IsAtBeginningOf(PageIteratorLevel.TextLine))
                {
                    currentLine = new LineModel();
                }

                var word = iterator.GetText(PageIteratorLevel.Word);
                if (!string.IsNullOrWhiteSpace(word))
                {
                    currentLine?.Words.Add(word);
                        
                    if (currentLineStringBuilder.Length > 0)
                        currentLineStringBuilder.Append(' ');
                    currentLineStringBuilder.Append(word);

                    if (currentBlockStringBuilder.Length > 0)
                        currentBlockStringBuilder.Append(' ');
                    currentBlockStringBuilder.Append(word);
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word) && currentLine?.Words.Count > 0)
                {
                    currentLine.Text = currentLineStringBuilder.ToString();
                    currentBlock?.Lines.Add(currentLine);
                    
                    currentLineStringBuilder.Clear();
                }

                if (iterator.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Word) && currentBlock?.Lines.Count > 0)
                {
                    currentBlock.Text = currentBlockStringBuilder.ToString();
                    blocks.Add(currentBlock);
                    
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
