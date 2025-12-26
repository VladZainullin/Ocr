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

        var maxOcr = Math.Max(1, Environment.ProcessorCount / 2);
        
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = cts.Token;

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
            try
            {
                await foreach (var aggTask in aggregatorChannel.Reader.ReadAllAsync(token))
                {
                    pages[aggTask.PageNumber - 1].Images.Add(aggTask.ImageModel);
                }
            }
            catch (Exception e)
            {
                imageChannel.Writer.TryComplete(e);
                aggregatorChannel.Writer.TryComplete(e);
                throw;
            }
        }, token);
        
        var workers = new Task[maxOcr];
        for (var i = 0; i < maxOcr; i++)
        {
            workers[i] = Task.Run(async () =>
            {
                try
                {
                    await foreach (var task in imageChannel.Reader.ReadAllAsync(token))
                    {
                        var bytes = imageService.Recognition(task.Image.Span);
                        if (bytes.Length == 0) continue;

                        var blocks = ocr.Process(bytes);
                        if (blocks.Count == 0) continue;
                    
                        var aggregated = new AggregatedTask(task.PageNumber, new ImageModel { Blocks = blocks });
                        await aggregatorChannel.Writer.WriteAsync(aggregated, token);
                    }
                }
                catch (Exception e)
                {
                    aggregatorChannel.Writer.TryComplete(e);
                    imageChannel.Writer.TryComplete(e);
                    throw;
                }
            }, token);
        }
        
        try
        {
            for (var pdfPageNumber = 1; pdfPageNumber <= pdf.NumberOfPages; pdfPageNumber++)
            {
                var page = pdf.GetPage(pdfPageNumber);
                pages[pdfPageNumber - 1] = new PageModel
                {
                    Number = page.Number,
                    Blocks = page.GetBlocks(),
                    Images = []
                };

                foreach (var pdfImage in page.GetImages())
                {
                    var memory = pdfImage.Memory();
                    if (memory.Length == 0) continue;

                    await imageChannel.Writer.WriteAsync(
                        new ImageTask(page.Number, memory), token);
                }
            }
        
            imageChannel.Writer.TryComplete();
            await Task.WhenAll(workers);

            aggregatorChannel.Writer.TryComplete();
            await aggregatorTask;
        }
        catch (Exception e)
        {
            if (!cts.IsCancellationRequested) cts.Cancel();
            aggregatorChannel.Writer.TryComplete(e);
            imageChannel.Writer.TryComplete(e);
            throw;
        }

        return new ResponseModel
        {
            Pages = pages
        };
    }
    
    private sealed record ImageTask(int PageNumber, ReadOnlyMemory<byte> Image);
    private sealed record AggregatedTask(int PageNumber, ImageModel ImageModel);
}
