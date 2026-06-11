using System.Text;
using Domain;
using Domain.Builders;
using Domain.Models;
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
        
        foreach (var textBlock in orderedBlocks)
        {
            if (textBlock.TextLines.Count == 0)
            {
                continue;
            }

            var blockModel = BuildBlock(textBlock, stringBuilderObjectPool);
            if (blockModel != null)
            {
                blockResponses.Add(blockModel);
            }
        }
        
        return blockResponses;
    }

    private static BlockModel? BuildBlock(
        UglyToad.PdfPig.DocumentLayoutAnalysis.TextBlock textBlock, 
        ObjectPool<StringBuilder> stringBuilderObjectPool)
    {
        using var blockBuilder = new BlockBuilder(stringBuilderObjectPool);
        
        foreach (var textLine in textBlock.TextLines)
        {
            var lineModel = BuildLine(textLine, stringBuilderObjectPool);
            if (lineModel != null)
            {
                blockBuilder.AddLine(lineModel);
            }
        }
        
        return blockBuilder.Build();
    }

    private static LineModel? BuildLine(
        UglyToad.PdfPig.DocumentLayoutAnalysis.TextLine textLine, 
        ObjectPool<StringBuilder> stringBuilderObjectPool)
    {
        if (textLine.Words.Count == 0)
        {
            return null;
        }

        using var lineBuilder = new LineBuilder(stringBuilderObjectPool);
        
        foreach (var word in textLine.Words)
        {
            lineBuilder.AddWord(word.Text);
        }
        
        return lineBuilder.Build();
    }
}