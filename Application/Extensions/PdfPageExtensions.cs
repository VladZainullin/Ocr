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
            
                var blockResponse = new BlockModel();
                foreach (var textLine in block.TextLines)
                {
                    var lineStringBuilder = stringBuilderObjectPool.Get();
                    try
                    {
                        if (textLine.Words.Count == 0) continue;
                
                        var line = new LineModel();
                        foreach (var word in textLine.Words)
                        {
                            line.Words.Add(word.Text);
                            lineStringBuilder.Append(word.Text);
                            lineStringBuilder.Append(' ');
                        }

                        lineStringBuilder.TrimEnd();
                        line.Text = lineStringBuilder.ToString();
                        
                        blockResponse.Lines.Add(line);
                        blockStringBuilder.Append(lineStringBuilder);
                        blockStringBuilder.Append(' ');
                    }
                    finally
                    {
                        stringBuilderObjectPool.Return(lineStringBuilder);
                    }
                }
                blockStringBuilder.TrimEnd();
                blockResponse.Text = blockStringBuilder.ToString();
                blockResponses.Add(blockResponse);
            }
            finally
            {
                stringBuilderObjectPool.Return(blockStringBuilder);
            }
        }
        
        return blockResponses;
    }
}