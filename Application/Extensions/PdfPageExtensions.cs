using Domain;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using Page = UglyToad.PdfPig.Content.Page;

namespace Application.Extensions;

public static class PdfPageExtensions
{
    public static BlockModel[] GetBlocks(this Page page)
    {
        var words = page.GetWords(NearestNeighbourWordExtractor.Instance);
        var blocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);
        var orderedBlocks = UnsupervisedReadingOrderDetector.Instance.Get(blocks);
        var blockResponses = new List<BlockModel>();
        foreach (var block in orderedBlocks)
        {
            if (block.TextLines.Count == 0) continue;
            
            var blockResponse = new BlockModel();
            foreach (var textLine in block.TextLines)
            {
                if (textLine.Words.Count == 0) continue;
                
                var line = new LineModel();
                foreach (var word in textLine.Words)
                {
                    line.Words.Add(word.Text);
                }
                blockResponse.Lines.Add(line);
            }
            blockResponses.Add(blockResponse);
        }
        
        return blockResponses.ToArray();
    }
}