using Domain;
using ImageService.Contracts;
using Ocr;
using UglyToad.PdfPig;
using Web.Extensions;

namespace Web.Services;

internal sealed class PdfService(OcrService ocr, IImageService imageService)
{
    public ResponseModel Process(Stream stream, ParallelOptions parallelOptions)
    {
        using var pdfDocument = PdfDocument.Open(stream);
        
        var pageResponses = new PageModel[pdfDocument.NumberOfPages];

        var batchSize = 100;
        for (var batchStart = 0; batchStart < pdfDocument.NumberOfPages; batchStart += batchSize)
        {
            var imageTextBuffers = new List<ImageTextBuffer>();
            var batchEnd = Math.Min(batchStart + batchSize, pdfDocument.NumberOfPages);
            for (var pdfPageNumber = batchStart + 1; pdfPageNumber <= batchEnd; pdfPageNumber++)
            {
                var pdfPage = pdfDocument.GetPage(pdfPageNumber);

                var blockResponses = pdfPage.GetBlocks();

                pageResponses[pdfPage.Number - 1] = new PageModel
                {
                    Number = pdfPage.Number,
                    Blocks = blockResponses,
                };

                foreach (var pdfImage in pdfPage.GetImages())
                {
                    var memory = pdfImage.Memory();
                    if (memory.Length == 0) continue;

                    imageTextBuffers.Add(new ImageTextBuffer
                    {
                        Number = pdfPage.Number,
                        Memory = memory
                    });
                }
            }

            Parallel.ForEach(imageTextBuffers, parallelOptions, imageTextBuffer =>
            {
                var bytes = imageService.Recognition(imageTextBuffer.Memory.Span);
                if (bytes.Length == 0) return;
                var blocks = ocr.Process(bytes);
                pageResponses[imageTextBuffer.Number - 1].Images.Add(new ImageModel
                {
                    Blocks = blocks,
                });
            });
        }

        return new ResponseModel
        {
            Pages = pageResponses
        };
    }
}

