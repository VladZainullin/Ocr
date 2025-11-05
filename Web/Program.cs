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

        app.MapPost("/documents", static async context =>
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
            using var pdfDocument = PdfDocument.Open(stream);
            
            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();

            var pageResponses = new ConcurrentBag<PageResponse>();
            
            var pages = pdfDocument.GetPages();

            foreach(var page in pages)
            {
                var searchableText = page.Text;

                var imageResponses = new ConcurrentBag<ImageResponse>();
                var pdfImages = page.GetImages();
                foreach(var pdfImage in pdfImages)
                {
                    if (pdfImage.IsInlineImage)
                    {
                        Console.WriteLine();
                    }

                    if (pdfImage.IsImageMask)
                    {
                        Console.WriteLine();
                    }
                    
                    if (pdfImage.RawBytes.Length == 0) continue;
                    var bytes = PreparateImage(pdfImage.RawBytes);
                    if (bytes.Length == 0) continue;

                    RecognitionTextFromImage(tesseractEngineObjectPool, bytes, imageResponses);
                }

                pageResponses.Add(new PageResponse
                {
                    Number = page.Number,
                    Text = searchableText,
                    Images = imageResponses,
                });
            }

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