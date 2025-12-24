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

        // 1️⃣ Инициализация PageModel
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

        // 2️⃣ Создаём bounded channel
        var maxOcr = Math.Max(1, Environment.ProcessorCount / 2);
        var channel = Channel.CreateBounded<ImageTask>(
            new BoundedChannelOptions(maxOcr)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

        // 3️⃣ OCR workers
        var workers = new Task[maxOcr];
        for (int i = 0; i < maxOcr; i++)
        {
            workers[i] = Task.Run(async () =>
            {
                await foreach (var task in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    await ProcessImageAsync(task, pages, cancellationToken);
                }
            }, cancellationToken);
        }

        // 4️⃣ Producer: читаем PDF последовательно
        for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
        {
            var page = pdf.GetPage(pageNumber);

            foreach (var pdfImage in page.GetImages())
            {
                var memory = pdfImage.Memory();
                if (memory.Length == 0) continue;

                await channel.Writer.WriteAsync(
                    new ImageTask(page.Number, memory), cancellationToken);
            }
        }

        // 5️⃣ Завершаем pipeline
        channel.Writer.Complete();
        await Task.WhenAll(workers);

        return new ResponseModel
        {
            Pages = pages
        };
    }

    private async Task ProcessImageAsync(
        ImageTask task,
        PageModel[] pages,
        CancellationToken cancellationToken)
    {
        // OCR / Recognition могут быть CPU или IO bound
        var bytes = await Task.Run(() => imageService.Recognition(task.Image.Span), cancellationToken);

        if (bytes.Length == 0) return;

        var blocks = await Task.Run(() => ocr.Process(bytes).ToList(), cancellationToken);

        // Добавление результата — критическая секция на странице
        lock (pages[task.PageNumber - 1])
        {
            pages[task.PageNumber - 1].Images.Add(
                new ImageModel { Blocks = blocks });
        }
    }

    private sealed record ImageTask(int PageNumber, ReadOnlyMemory<byte> Image);
}
