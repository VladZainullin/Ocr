using System.Diagnostics;
using System.Threading.Channels;
using Application.Contracts;
using Application.Extensions;
using Domain.Builders;
using Domain.Models;
using ImageService.Contracts;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using UglyToad.PdfPig;

namespace Application;

internal sealed class PdfService(
    IOcrService ocr,
    IImageService imageService,
    ObjectPool<PageBuilder> pageBuilderObjectPool,
    ObjectPool<BlockBuilder> blockBuilderObjectPool,
    ObjectPool<LineBuilder> lineBuilderObjectPool,
    ParsingOptions parsingOptions,
    ActivitySource activitySource) : IPdfService
{
    public async Task<ResponseModel> ProcessAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var activity = activitySource.StartActivity();
        using var pdf = PdfDocument.Open(stream, parsingOptions);

        var pageBuilders = new List<PageBuilder>(pdf.NumberOfPages);
        for (var index = 0; index < pdf.NumberOfPages; index++)
        {
            var pageBuilder = pageBuilderObjectPool.Get();
            pageBuilders.Add(pageBuilder);
        }
        
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

        try
        {
            var aggregatorTask = AddRecognitionTextToResponse(pageBuilders, aggregatorChannel, imageChannel, token);

            var workers = new Task[maxOcr];
            for (var i = 0; i < maxOcr; i++)
            {
                workers[i] = RecognitionImageAsync(imageChannel, aggregatorChannel, token);
            }

            await ProcessDocument(pdf, pageBuilders, imageChannel, token);

            imageChannel.Writer.TryComplete();
            await Task.WhenAll(workers);

            aggregatorChannel.Writer.TryComplete();
            await aggregatorTask;
            
            return new ResponseModel
            {
                Pages = pageBuilders.Select(s => s.Build()).ToList()
            };
        }
        catch (Exception e)
        {
            if (!cts.IsCancellationRequested) await cts.CancelAsync();
            aggregatorChannel.Writer.TryComplete(e);
            imageChannel.Writer.TryComplete(e);
            throw;
        }
        finally
        {
            foreach (var pageBuilder in pageBuilders)
            {
                pageBuilderObjectPool.Return(pageBuilder);
            }
        }
    }

    private async Task ProcessDocument(
        PdfDocument pdf,
        List<PageBuilder> pageBuilders,
        Channel<RecognitionImageTask> imageChannel,
        CancellationToken cancellationToken)
    {
        for (var pdfPageNumber = 1; pdfPageNumber <= pdf.NumberOfPages; pdfPageNumber++)
        {
            var page = pdf.GetPage(pdfPageNumber);

            var pageBuilder = pageBuilders[pdfPageNumber - 1];

            pageBuilder.SetNumber(page.Number);

            var blocks = page.GetBlocks(blockBuilderObjectPool, lineBuilderObjectPool);
            foreach (var block in blocks)
            {
                pageBuilder.AddBlock(block);
            }

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
        List<PageBuilder> pages,
        Channel<AggregatedImageTask> aggregatorChannel,
        Channel<RecognitionImageTask> imageChannel,
        CancellationToken token)
    {
        try
        {
            await foreach (var aggTask in aggregatorChannel.Reader.ReadAllAsync(token))
            {
                var pageBuilder = pages[aggTask.PageNumber - 1];

                pageBuilder.AddImage(aggTask.ImageModel);
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
                if (!imageService.TryPrepare(task.Image.Span, out var data, out var width, out var height,
                        out var bytesPerPixel)) continue;
                
                var imageModel = ocr.Recognition(data, width, height, bytesPerPixel);
                if (ReferenceEquals(imageModel, null)) continue;

                var aggregated = new AggregatedImageTask(task.PageNumber, imageModel);
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