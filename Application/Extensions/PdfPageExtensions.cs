using System.Text;
using Domain;
using Microsoft.Extensions.ObjectPool;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using Page = UglyToad.PdfPig.Content.Page;

namespace Application.Extensions;

public static class PdfPageExtensions
{
    public static List<BlockModel> GetBlocks(this Page page, ObjectPool<StringBuilder> stringBuilderObjectPool)
    {
        var words = page.GetWords(NearestNeighbourWordExtractor.Instance);
        var blocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);
        var orderedBlocks = UnsupervisedReadingOrderDetector.Instance.Get(blocks);
        var blockResponses = new List<BlockModel>(blocks.Count);
        foreach (var block in orderedBlocks)
        {
            var blockStringBuilder = stringBuilderObjectPool.Get();
            try
            {
                if (block.TextLines.Count == 0) continue;
            
                var textLines = new List<LineModel>();
                foreach (var textLine in block.TextLines)
                {
                    var lineStringBuilder = stringBuilderObjectPool.Get();
                    try
                    {
                        if (textLine.Words.Count == 0) continue;
                
                        var blockWords = new List<string>();
                        foreach (var word in textLine.Words)
                        {
                            blockWords.Add(word.Text);
                            lineStringBuilder.Append(word.Text);
                            lineStringBuilder.Append(' ');
                        }

                        lineStringBuilder.TrimEnd();
                        
                        textLines.Add(new LineModel
                        {
                            Text = lineStringBuilder.ToString(),
                            Words = blockWords,
                        });
                        blockStringBuilder.Append(lineStringBuilder);
                        blockStringBuilder.Append(' ');
                    }
                    finally
                    {
                        stringBuilderObjectPool.Return(lineStringBuilder);
                    }
                }
                blockStringBuilder.TrimEnd();
                blockResponses.Add(new BlockModel
                {
                    Text = blockStringBuilder.ToString(),
                    Lines = textLines
                });
            }
            finally
            {
                stringBuilderObjectPool.Return(blockStringBuilder);
            }
        }
        
        return blockResponses;
    }
}