using Domain;
using ImageService.Contracts;
using OcrService.Contracts;
using UglyToad.PdfPig;
using Web.Extensions;

namespace Web.Services;

internal sealed class PdfService(IOcrService ocr, IImageService imageService)
{
    public ResponseModel Process(Stream stream)
    {
        using var pdf = PdfDocument.Open(stream);

        var pages = new PageModel[pdf.NumberOfPages];
        
        var maxOcr = Math.Min(Math.Max(1, Environment.ProcessorCount - 1), 16);

        var runningTasks = new List<Task>();

        for (var pdfPageNumber = 1; pdfPageNumber <= pdf.NumberOfPages; pdfPageNumber++)
        {
            var page = pdf.GetPage(pdfPageNumber);

            var pageModel = new PageModel
            {
                Number = page.Number,
                Blocks = page.GetBlocks(),
                Images = []
            };

            pages[page.Number - 1] = pageModel;

            foreach (var pdfImage in page.GetImages())
            {
                var memory = pdfImage.Memory();
                if (memory.Length == 0)
                    continue;
                
                var task = Task.Run(() => { ProcessImage(memory, pageModel); });

                runningTasks.Add(task);

                if (runningTasks.Count < maxOcr) continue;
                
                var finished = Task.WaitAny(runningTasks.ToArray());
                runningTasks.RemoveAt(finished);
            }
        }
        
        Task.WaitAll(runningTasks);

        return new ResponseModel
        {
            Pages = pages
        };
    }

    private void ProcessImage(
        ReadOnlyMemory<byte> image,
        PageModel page)
    {
        var bytes = imageService.Recognition(image.Span);
        if (bytes.Length == 0)
            return;

        var blocks = ocr.Process(bytes).ToList();

        lock (page)
        {
            page.Images.Add(new ImageModel
            {
                Blocks = blocks
            });
        }
    }
}