using System.Buffers;
using System.Net.Mime;
using ImageMagick;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using Page = Tesseract.Page;

namespace Web;

file sealed class Program
{
    public static readonly ParallelOptions ParallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(static options => options.Limits.MaxRequestBodySize = 100 * 1024 * 1024);

        builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        builder.Services.TryAddSingleton<ObjectPool<TesseractEngine>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = new TesseractEnginePooledObjectPolicy("./tesseract", "rus+eng");
            return provider.Create(policy);
        });

        await using var app = builder.Build();

        app.MapPost("api/v3/documents", static async context =>
        {
            if (context.Request.Form.Files.Count < 1
                || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf
                || context.Request.Form.Files[0].Length == 0)
            {
                await context.Response.WriteAsJsonAsync(new Response
                {
                    Pages = [],
                });
                return;
            }

            await using var stream = context.Request.Form.Files[0].OpenReadStream();
            using var pdfDocument = PdfDocument.Open(stream);

            var searchTextBuffers = new SearchTextBuffer[pdfDocument.NumberOfPages];

            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();

            var batchSize = 100;
            for (var batchStart = 0; batchStart < pdfDocument.NumberOfPages; batchStart += batchSize)
            {
                var imageTextBuffers = new List<ImageTextBuffer>();
                var batchEnd = Math.Min(batchStart + batchSize, pdfDocument.NumberOfPages);
                for (var pdfPageNumber = batchStart + 1; pdfPageNumber <= batchEnd; pdfPageNumber++)
                {
                    var pdfPage = pdfDocument.GetPage(pdfPageNumber);
                    
                    searchTextBuffers[pdfPage.Number - 1] = new SearchTextBuffer
                    {
                        Number = pdfPage.Number,
                        Text = pdfPage.Text,
                    };

                    foreach (var pdfImage in pdfPage.GetImages())
                    {
                        var imageBytes = GetImageBytes(pdfImage);
                        if (imageBytes.Length == 0) continue;

                        imageTextBuffers.Add(new ImageTextBuffer
                        {
                            Number = pdfPage.Number,
                            Memory = imageBytes.ToArray(),
                        });
                    }
                }

                Parallel.ForEach(imageTextBuffers, new ParallelOptions
                {
                    CancellationToken = context.RequestAborted,
                    MaxDegreeOfParallelism = Math.Min(Math.Max(1, Environment.ProcessorCount - 1), 16),
                }, imageTextBuffer =>
                {
                    var engine = tesseractEngineObjectPool.Get();
                    try
                    {
                        var preparateImage = PreparateImage(imageTextBuffer.Memory);
                        if (preparateImage.Length == 0) return;
                        using var imageDocument = Pix.LoadFromMemory(preparateImage);
                        using var imagePage = engine.Process(imageDocument);
                        Console.WriteLine(imagePage.GetMeanConfidence());
                        var blocks = ExtractLayoutFromPage(imagePage);
                        var text = imagePage.GetText();
                        if (text == string.Empty) return;
                        searchTextBuffers[imageTextBuffer.Number - 1].Images.Add(new ImageResponse
                        {
                            Blocks = blocks,
                        });
                    }
                    finally
                    {
                        tesseractEngineObjectPool.Return(engine);
                    }
                });
            }

            await context.Response.WriteAsJsonAsync(new
            {
                Pages = searchTextBuffers
            });
        });

        app.MapPost("api/v1/documents", static async context =>
        {
            if (context.Request.Form.Files.Count < 1
                || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf
                || context.Request.Form.Files[0].Length == 0)
            {
                await context.Response.WriteAsJsonAsync(new Response
                {
                    Pages = [],
                });
                return;
            }

            await using var stream = context.Request.Form.Files[0].OpenReadStream();

            using var pdfDocument = PdfDocument.Open(stream);

            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();

            var pdfPages = pdfDocument.GetPages();

            var pageResponses = ArrayPool<PageResponse>.Shared.Rent(pdfDocument.NumberOfPages);
            try
            {
                foreach (var pdfPage in pdfPages)
                {
                    var pdfImages = pdfPage.GetImages();
                    var imageResponses = new List<string>();
                    foreach (var pdfImage in pdfImages)
                    {
                        var imageBytes = GetImageBytes(pdfImage);
                        if (imageBytes.Length == 0) continue;
                        var preparateImageBytes = PreparateImage(imageBytes);
                        if (preparateImageBytes.Length == 0) continue;
                        using var imageDocument = Pix.LoadFromMemory(preparateImageBytes);
                        var engine = tesseractEngineObjectPool.Get();
                        try
                        {
                            using var imagePage = engine.Process(imageDocument);
                            var blocks = ExtractLayoutFromPage(imagePage);
                        }
                        finally
                        {
                            tesseractEngineObjectPool.Return(engine);
                        }
                    }

                    pageResponses[pdfPage.Number - 1] = new PageResponse
                    {
                        Number = pdfPage.Number,
                        Text = pdfPage.Text,
                        Images = imageResponses
                    };
                }

                await context.Response.WriteAsJsonAsync(new Response
                {
                    Pages = pageResponses,
                });
            }
            finally
            {
                ArrayPool<PageResponse>.Shared.Return(pageResponses);
            }
        });

        await app.RunAsync();
    }

    private static List<BlockResponse> ExtractLayoutFromPage(Page page)
    {
        var blocks = new List<BlockResponse>();
        using var iter = page.GetIterator();
        iter.Begin();

        do
        {
            var block = new BlockResponse();
            do
            {
                var paragraph = new ParagraphResponse();
                do
                {
                    var text = iter.GetText(PageIteratorLevel.TextLine);
                    paragraph.Lines.Add(text);
                } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                block.Paragraphs.Add(paragraph);
            } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));

            blocks.Add(block);
        } while (iter.Next(PageIteratorLevel.Block));

        return blocks;
    }

    private static Span<byte> GetImageBytes(IPdfImage pdfImage)
    {
        if (pdfImage.TryGetPng(out var pngImageBytes))
        {
            return pngImageBytes;
        }

        if (pdfImage.TryGetBytesAsMemory(out var memory))
        {
            return memory.Span;
        }

        return pdfImage.RawBytes;
    }

    private static byte[] PreparateImage(Span<byte> bytes)
    {
        try
        {
            using var image = new MagickImage(bytes);
            // 1. Конвертация в оттенки серого
            image.Grayscale();

            // 2. Увеличение разрешения (если нужно)
            image.Resize(new Percentage(200)); // Увеличение в 2 раза

            // 3. Выравнивание изображения (автоматическая ориентация)
            image.AutoOrient();

            // 4. Удаление шумов
            image.Despeckle();

            // 7. Обрезка пустых полей
            image.Trim();

            // 8. Установка DPI
            image.Density = new Density(300, 300);

            return image.ToByteArray();
        }
        catch (MagickMissingDelegateErrorException)
        {
            return [];
        }
    }

    private static byte[] PreparateImage(Stream stream)
    {
        try
        {
            using var image = new MagickImage(stream);
            image.Grayscale();
            image.Strip();
            return image.ToByteArray();
        }
        catch (MagickMissingDelegateErrorException)
        {
            return [];
        }
    }
}

public sealed class ImageResponse
{
    public required List<BlockResponse> Blocks { get; set; }
}

public sealed class BlockResponse
{
    public List<ParagraphResponse> Paragraphs { get; } = [];
}

public sealed class ParagraphResponse
{
    public List<string> Lines { get; } = [];
}

public sealed class SearchTextBuffer
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public List<ImageResponse> Images { get; } = [];
}

public sealed class ImageTextBuffer
{
    public required int Number { get; init; }

    public required byte[] Memory { get; init; }
}

public sealed class Response
{
    public required IReadOnlyCollection<PageResponse> Pages { get; init; }
}

public sealed class PageResponse
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public required List<string> Images { get; set; }
}