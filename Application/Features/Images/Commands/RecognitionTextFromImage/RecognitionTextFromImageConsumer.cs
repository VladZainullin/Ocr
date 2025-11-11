using Application.Contracts.Features.Images.Commands.RecognitionTextFromImage;
using ImageMagick;
using MediatR;
using Tesseract;

namespace Application.Features.Images.Commands.RecognitionTextFromImage;

internal sealed class RecognitionTextFromImageConsumer : IRequestHandler<RecognitionTextFromImageRequest, RecognitionTextFromImageResponse>
{
    public Task<RecognitionTextFromImageResponse> Handle(RecognitionTextFromImageRequest request, CancellationToken cancellationToken)
    {
        using var image = new MagickImage(request.Form.Stream);

        image.AutoOrient();
        image.Trim();

        image.Grayscale();

        image.MedianFilter(1);

        var bytes = image.ToByteArray();

        using var tesseract = new TesseractEngine("./tesseract", "eng+rus");
        var pix = Pix.LoadFromMemory(bytes);
        var page = tesseract.Process(pix);
        var text = page.GetText();
        var blocks = ExtractLayoutFromPage(page);

        return Task.FromResult(new RecognitionTextFromImageResponse
        {
            Blocks = blocks
        });
    }
    
    private static List<Block> ExtractLayoutFromPage(Page page)
    {
        var blocks = new List<Block>();
        using var iter = page.GetIterator();
        iter.Begin();
        Block currentBlock = null!;
        Block.Paragraph currentParagraph = null!;
        Block.Line currentLine = null!;
                
        do
        {
            if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
            {
                currentBlock = new Block();
            }

            if (iter.IsAtBeginningOf(PageIteratorLevel.Para))
            {
                currentParagraph = new Block.Paragraph();
                currentBlock.Paragraphs.Add(currentParagraph);
            }

            if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine))
            {
                currentLine = new Block.Line();
                currentParagraph.Lines.Add(currentLine);
            }

            if (iter.IsAtBeginningOf(PageIteratorLevel.Word))
            {
                var word = iter.GetText(PageIteratorLevel.Word);
                currentLine.Words.Add(word);
            }

            if (!iter.IsAtFinalOf(PageIteratorLevel.Word, PageIteratorLevel.Word)) continue;
            if (!iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word)) continue;
            if (!iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine)) continue;
            if (!iter.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Para)) continue;
            blocks.Add(currentBlock);
        } while (iter.Next(PageIteratorLevel.Word));

        return blocks;
    }
}