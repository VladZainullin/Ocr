using System.Collections.Concurrent;
using ImageMagick;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Tesseract;
using UglyToad.PdfPig;

namespace Web;

file sealed class Program
{
    private static readonly ParallelOptions ParallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(static options => options.Limits.MaxRequestBodySize = 50 * 1024 * 1024);

        builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        builder.Services.TryAddSingleton<ObjectPool<TesseractEngine>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = new TesseractEnginePooledObjectPolicy("./tesseract", "rus");
            return provider.Create(policy);
        });

        await using var app = builder.Build();

        app.MapPost("api/v2/documents", static async context =>
        {
            if (context.Request.Form.Files.Count < 1)
            {
                await context.Response.WriteAsJsonAsync(new Response
                {
                    Pages = [],
                });
                return;
            }

            await using var stream = context.Request.Form.Files[0].OpenReadStream();
            
            var bytes = new byte[stream.Length];
            await stream.ReadExactlyAsync(bytes);
            
            using var pdfDocument = PdfDocument.Open(bytes);
            var pdfPageWithImagesNumbers = pdfDocument
                .GetPages()
                .Select(p => p.Number)
                .ToArray();
            
            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();

            var pageResponses = new ConcurrentBag<PageResponse>();
            
            Parallel.ForEach(pdfPageWithImagesNumbers, ParallelOptions, pdfPageNumber =>
            {
                using var pdfDocument = PdfDocument.Open(bytes);

                var pdfPage = pdfDocument.GetPage(pdfPageNumber);
                var pdfImages = pdfPage.GetImages();
                var imageResponses = new ConcurrentBag<ImageResponse>();
                foreach (var pdfImage in pdfImages)
                {
                    var bytes = PreparateImage(pdfImage.RawBytes);
                    if (bytes.Length == 0) continue;
                    var engine = tesseractEngineObjectPool.Get();
                    try
                    {
                        using var imageDocument = Pix.LoadFromMemory(bytes);
                        using var imagePage = engine.Process(imageDocument);
                        var text = imagePage.GetText();
                        imageResponses.Add(new ImageResponse
                        {
                            Text = text
                        });
                    }
                    finally
                    {
                        tesseractEngineObjectPool.Return(engine);
                    }
                }
                
                pageResponses.Add(new PageResponse
                {
                    Number = pdfPage.Number,
                    Text = pdfPage.Text,
                    Images = imageResponses,
                });
            });
            
            await context.Response.WriteAsJsonAsync(new Response
            {
                Pages = pageResponses,
            });
        });

        await app.RunAsync();
    }

    private static void RecognitionTextFromImage(
        ObjectPool<TesseractEngine> tesseractEngineObjectPool,
        byte[] bytes,
        ConcurrentBag<ImageResponse> imageResponses)
    {
        var engine = tesseractEngineObjectPool.Get();
        try
        {
            using var imageDocument = Pix.LoadFromMemory(bytes);
            using var imagePage = engine.Process(imageDocument);
            var text = imagePage.GetText();
            imageResponses.Add(new ImageResponse
            {
                Text = text
            });
        }
        finally
        {
            tesseractEngineObjectPool.Return(engine);
        }
    }

    private static byte[] PreparateImage(Span<byte> bytes)
    {
        using var image = new MagickImage();
        try
        {
            image.Read(bytes);
            image.AutoOrient();
            image.Despeckle();
            image.Grayscale();
            return image.ToByteArray();
        }
        catch (MagickMissingDelegateErrorException)
        {
            return [];
        }
    }
}

public sealed class Response
{
    public required IEnumerable<PageResponse> Pages { get; init; }
}

public sealed class PageResponse
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public required IEnumerable<ImageResponse> Images { get; init; }
}

public sealed class ImageResponse
{
    public required string Text { get; init; }
}