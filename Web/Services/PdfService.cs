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

        // 1️⃣ Инициализация PageModel
        var pages = new PageModel[pdf.NumberOfPages];
        for (int i = 1; i <= pdf.NumberOfPages; i++)
        {
            var page = pdf.GetPage(i);
            pages[i - 1] = new PageModel
            {
                Number = page.Number,
                Blocks = page.GetBlocks(),
                Images = new List<ImageModel>()
            };
        }

        // 2️⃣ Каналы: один для изображений, один для aggregator
        var maxOcr = Math.Max(1, Environment.ProcessorCount / 2);

        var imageChannel = Channel.CreateBounded<ImageTask>(
            new BoundedChannelOptions(maxOcr)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

        var aggregatorChannel = Channel.CreateUnbounded<AggregatedTask>();

        // 3️⃣ Aggregator task
        var aggregatorTask = Task.Run(async () =>
        {
            await foreach (var aggTask in aggregatorChannel.Reader.ReadAllAsync(cancellationToken))
            {
                // без lock, т.к. только aggregator пишет в PageModel
                pages[aggTask.PageNumber - 1].Images.Add(aggTask.ImageModel);
            }
        }, cancellationToken);

        // 4️⃣ OCR worker tasks
        var workers = new Task[maxOcr];
        for (int i = 0; i < maxOcr; i++)
        {
            workers[i] = Task.Run(async () =>
            {
                await foreach (var task in imageChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    var bytes = await Task.Run(() => imageService.Recognition(task.Image.Span), cancellationToken);
                    if (bytes.Length == 0) continue;

                    var blocks = await Task.Run(() => ocr.Process(bytes).ToList(), cancellationToken);

                    // Отправляем в aggregator
                    var aggregated = new AggregatedTask(task.PageNumber, new ImageModel { Blocks = blocks });
                    await aggregatorChannel.Writer.WriteAsync(aggregated, cancellationToken);
                }
            }, cancellationToken);
        }

        // 5️⃣ Producer: читаем PDF последовательно
        for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
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

        // 6️⃣ Завершаем каналы
        imageChannel.Writer.Complete();
        await Task.WhenAll(workers);

        aggregatorChannel.Writer.Complete();
        await aggregatorTask;

        return new ResponseModel
        {
            Pages = pages
        };
    }

    // DTO для OCR pipeline
    private sealed record ImageTask(int PageNumber, ReadOnlyMemory<byte> Image);
    private sealed record AggregatedTask(int PageNumber, ImageModel ImageModel);
}
