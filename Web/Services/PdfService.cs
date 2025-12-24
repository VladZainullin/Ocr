using System.Threading.Channels;
using Domain;
using ImageService.Contracts;
using OcrService.Contracts;
using UglyToad.PdfPig;
using Web.Extensions;

namespace Web.Services;

internal sealed class PdfService(IOcrService ocr, IImageService imageService)
{
    public async Task<ResponseModel> Process(Stream stream)
    {
        using var pdf = PdfDocument.Open(stream);

        var pages = new PageModel[pdf.NumberOfPages];

        // 1️⃣ Создаём страницы сразу (без concurrency)
        for (int pdfPageNumber = 1; pdfPageNumber <= pdf.NumberOfPages; pdfPageNumber++)
        {
            var page = pdf.GetPage(pdfPageNumber);
            pages[pdfPageNumber - 1] = new PageModel
            {
                Number = page.Number,
                Blocks = page.GetBlocks(),
                Images = new List<ImageModel>()
            };
        }

        // 2️⃣ Bounded channel = контроль памяти
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
                await foreach (var task in channel.Reader.ReadAllAsync())
                {
                    ProcessImage(task, pages);
                }
            });
        }

        // 4️⃣ Producer — строго последовательно
        for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
        {
            var page = pdf.GetPage(pageNumber);

            foreach (var pdfImage in page.GetImages())
            {
                var memory = pdfImage.Memory();
                if (memory.Length == 0)
                    continue;

                await channel.Writer.WriteAsync(
                    new ImageTask(page.Number, memory)
                );
            }
        }

        // 5️⃣ Завершаем pipeline
        channel.Writer.Complete();
        Task.WaitAll(workers);

        return new ResponseModel
        {
            Pages = pages
        };
    }

    private void ProcessImage(ImageTask task, PageModel[] pages)
    {
        var bytes = imageService.Recognition(task.Image.Span);
        if (bytes.Length == 0)
            return;

        var blocks = ocr.Process(bytes).ToList();

        // 👇 только одна точка записи, без lock
        pages[task.PageNumber - 1].Images.Add(
            new ImageModel { Blocks = blocks }
        );
    }
}

internal sealed record ImageTask(
    int PageNumber,
    ReadOnlyMemory<byte> Image
);