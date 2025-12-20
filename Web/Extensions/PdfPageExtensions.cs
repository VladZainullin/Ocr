using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using Web.Models;
using Page = UglyToad.PdfPig.Content.Page;

namespace Web.Extensions;

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
            var blockResponse = new BlockModel();
            foreach (var textLine in block.TextLines)
            {
                var line = new LineModel();
                foreach (var word in textLine.Words)
                {
                    line.Words.Add(word.Text);
                }

                if (line.Words.Count > 0)
                {
                    blockResponse.Lines.Add(line);
                }
            }

            if (blockResponse.Lines.Count > 0)
            {
                blockResponses.Add(blockResponse);
            }
        }
        
        return blockResponses.ToArray();
    }
}