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
    public static List<BlockModel> GetBlocks(this Page page, 
        ObjectPool<BlockBuilder> blockBuilderObjectPool,
        ObjectPool<LineBuilder> lineBuilderObjectPool)
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

            var blockModel = BuildBlock(
                textBlock,
                blockBuilderObjectPool,
                lineBuilderObjectPool);
            
            if (blockModel != null)
            {
                blockResponses.Add(blockModel);
            }
        }
        
        return blockResponses;
    }

    private static BlockModel? BuildBlock(
        UglyToad.PdfPig.DocumentLayoutAnalysis.TextBlock textBlock,
        ObjectPool<BlockBuilder> blockBuilderObjectPool,
        ObjectPool<LineBuilder> lineBuilderObjectPool)
    {
        var blockBuilder = blockBuilderObjectPool.Get();
        try
        {
            foreach (var textLine in textBlock.TextLines)
            {
                var lineModel = BuildLine(textLine, lineBuilderObjectPool);
                if (lineModel != null)
                {
                    blockBuilder.AddLine(lineModel);
                }
            }
        
            return blockBuilder.Build();
        }
        finally
        {
            blockBuilderObjectPool.Return(blockBuilder);
        }
    }

    private static LineModel? BuildLine(
        UglyToad.PdfPig.DocumentLayoutAnalysis.TextLine textLine, 
        ObjectPool<LineBuilder> lineBuilderObjectPool)
    {
        if (textLine.Words.Count == 0)
        {
            return null;
        }

        var lineBuilder = lineBuilderObjectPool.Get();
        try
        {
            foreach (var word in textLine.Words)
            {
                lineBuilder.AddWord(word.Text);
            }
        
            return lineBuilder.Build();   
        }
        finally
        {
            lineBuilderObjectPool.Return(lineBuilder);
        }
    }
}