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
    public List<BlockModel> Process(byte[] bytes)
    {
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
                        
                        currentLineStringBuilder.Append(word);
                        currentLineStringBuilder.Append(' ');
                        
                        currentBlockStringBuilder.Append(word);
                        currentBlockStringBuilder.Append(' ');
                    }
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
            tesseractEngineObjectPool.Return(tesseractEngine);
        }

        return blocks;
    }
}