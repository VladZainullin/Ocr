using System.Threading.Channels;
using Domain;
using ImageService.Contracts;
using OcrService.Contracts;
using UglyToad.PdfPig;
using Web.Extensions;

namespace Web.Services;

internal sealed class PdfService(IOcrService ocr, IImageService imageService)
{
    public async Task<ResponseModel> ProcessAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var pdf = PdfDocument.Open(stream);
        
        var pages = new PageModel[pdf.NumberOfPages];
        for (var pdfPageNumber = 1; pdfPageNumber <= pdf.NumberOfPages; pdfPageNumber++)
        {
            var page = pdf.GetPage(pdfPageNumber);
            pages[pdfPageNumber - 1] = new PageModel
            {
                Number = page.Number,
                Blocks = page.GetBlocks(),
                Images = []
            };
        }

        var maxOcr = Math.Max(1, Environment.ProcessorCount / 2);

        var imageChannel = Channel.CreateBounded<ImageTask>(new BoundedChannelOptions(maxOcr)
        {
            SingleWriter = true
        });

        var aggregatorChannel = Channel.CreateUnbounded<AggregatedTask>(new UnboundedChannelOptions
        {
            SingleReader = true
        });
        
        var aggregatorTask = Task.Run(async () =>
        {
            await foreach (var aggTask in aggregatorChannel.Reader.ReadAllAsync(cancellationToken))
            {
                pages[aggTask.PageNumber - 1].Images.Add(aggTask.ImageModel);
            }
        }, cancellationToken);
        
        var workers = new Task[maxOcr];
        for (var i = 0; i < maxOcr; i++)
        {
            workers[i] = Task.Run(async () =>
            {
                await foreach (var task in imageChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    var bytes = imageService.Recognition(task.Image.Span);
                    if (bytes.Length == 0) continue;

                    var blocks = ocr.Process(bytes);
                    if (blocks.Count == 0) continue;
                    
                    var aggregated = new AggregatedTask(task.PageNumber, new ImageModel { Blocks = blocks });
                    await aggregatorChannel.Writer.WriteAsync(aggregated, cancellationToken);
                }
            }, cancellationToken);
        }
        
        for (var pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
        {
            var page = pdf.GetPage(pageNumber);

            foreach (var pdfImage in page.GetImages())
            {
                var memory = pdfImage.Memory();
                if (memory.Length == 0) continue;

                await imageChannel.Writer.WriteAsync(
                    new ImageTask(page.Number, memory), cancellationToken);
            }
        }
        
        imageChannel.Writer.Complete();
        await Task.WhenAll(workers);

        aggregatorChannel.Writer.Complete();
        await aggregatorTask;

        return new ResponseModel
        {
            Pages = pages
        };
    }
    
    private sealed record ImageTask(int PageNumber, ReadOnlyMemory<byte> Image);
    private sealed record AggregatedTask(int PageNumber, ImageModel ImageModel);
}
