using System.Text;
using System.Threading.Channels;
using Application.Contracts;
using Application.Extensions;
using Domain;
using ImageService.Contracts;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using UglyToad.PdfPig;

namespace Application;

internal sealed class PdfService(IOcrService ocr, IImageService imageService, 
    ObjectPool<StringBuilder> stringBuilderObjectPool) : IPdfService
{
    public async Task<ResponseModel> ProcessAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var pdf = PdfDocument.Open(stream);
        
        var pages = new PageModel[pdf.NumberOfPages];

        var maxOcr = Math.Max(1, Environment.ProcessorCount / 2);
        
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = cts.Token;

        var imageChannel = Channel.CreateBounded<RecognitionImageTask>(new BoundedChannelOptions(maxOcr)
        {
            SingleWriter = true
        });

        var aggregatorChannel = Channel.CreateUnbounded<AggregatedImageTask>(new UnboundedChannelOptions
        {
            SingleReader = true
        });
        
        var aggregatorTask = AddRecognitionTextToResponse(pages, aggregatorChannel, imageChannel, token);
        
        var workers = new Task[maxOcr];
        for (var i = 0; i < maxOcr; i++)
        {
            workers[i] = RecognitionImageAsync(imageChannel, aggregatorChannel, token);
        }
        
        try
        {
            await ProcessDocument(pdf, pages, imageChannel, token);
        
            imageChannel.Writer.TryComplete();
            await Task.WhenAll(workers);

            aggregatorChannel.Writer.TryComplete();
            await aggregatorTask;
        }
        catch (Exception e)
        {
            if (!cts.IsCancellationRequested) await cts.CancelAsync();
            aggregatorChannel.Writer.TryComplete(e);
            imageChannel.Writer.TryComplete(e);
            throw;
        }

        return new ResponseModel
        {
            Pages = pages
        };
    }

    private async Task ProcessDocument(
        PdfDocument pdf,
        PageModel[] pages,
        Channel<RecognitionImageTask> imageChannel,
        CancellationToken cancellationToken)
    {
        for (var pdfPageNumber = 1; pdfPageNumber <= pdf.NumberOfPages; pdfPageNumber++)
        {
            var page = pdf.GetPage(pdfPageNumber);
            pages[pdfPageNumber - 1] = new PageModel
            {
                Number = page.Number,
                Blocks = page.GetBlocks(stringBuilderObjectPool),
                Images = []
            };

            foreach (var pdfImage in page.GetImages())
            {
                var memory = pdfImage.Memory();
                if (memory.Length == 0) continue;

                await imageChannel.Writer.WriteAsync(
                    new RecognitionImageTask(page.Number, memory), cancellationToken);
            }
        }
    }

    private static async Task AddRecognitionTextToResponse(
        PageModel[] pages,
        Channel<AggregatedImageTask> aggregatorChannel,
        Channel<RecognitionImageTask> imageChannel,
        CancellationToken token)
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
    }

    private async Task RecognitionImageAsync(
        Channel<RecognitionImageTask> imageChannel, 
        Channel<AggregatedImageTask> aggregatorChannel, 
        CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var task in imageChannel.Reader.ReadAllAsync(cancellationToken))
            {
                var bytes = imageService.Prepare(task.Image.Span);
                if (bytes.Length == 0) continue;

                var blocks = ocr.Recognition(bytes);
                if (blocks.Count == 0) continue;
                    
                var aggregated = new AggregatedImageTask(task.PageNumber, new ImageModel { Blocks = blocks });
                await aggregatorChannel.Writer.WriteAsync(aggregated, cancellationToken);
            }
        }
        catch (Exception e)
        {
            aggregatorChannel.Writer.TryComplete(e);
            imageChannel.Writer.TryComplete(e);
            throw;
        }
    }

    private sealed record RecognitionImageTask(int PageNumber, ReadOnlyMemory<byte> Image);
    private sealed record AggregatedImageTask(int PageNumber, ImageModel ImageModel);
}
