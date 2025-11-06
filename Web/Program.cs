using System.Collections.Concurrent;
using System.Net.Mime;
using ImageMagick;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Web;

file sealed class Program
{
    private static readonly ParallelOptions ParallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount,
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

        app.MapPost("api/v1/documents", static async context =>
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

            var streamBytes = new byte[stream.Length];
            await stream.ReadExactlyAsync(streamBytes);

            using var pdfDocument = PdfDocument.Open(streamBytes);
            var pdfPages = pdfDocument.GetPages();

            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();

            var pageResponses = new ConcurrentBag<PageResponse>();

            foreach (var pdfPage in pdfPages)
            {
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
            }

            await context.Response.WriteAsJsonAsync(new Response
            {
                Pages = pageResponses.OrderBy(static response => response.Number),
            });
        });

        app.MapPost("api/v2/documents", static async context =>
        {
            if (context.Request.Form.Files.Count < 1 
                || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf)
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
            
            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();
            var objectPoolProvider = context.RequestServices.GetRequiredService<ObjectPoolProvider>();
            var pdfDocumentObjectPool = objectPoolProvider.Create(new PdfDocumentPooledObjectPolicy(bytes));
            
            var pdfDocument = pdfDocumentObjectPool.Get();
            var pdfDocumentNumberOfPage = pdfDocument.NumberOfPages;
            pdfDocumentObjectPool.Return(pdfDocument);

            var pageResponses = new ConcurrentBag<PageResponse>();
            
            Parallel.For(1, pdfDocumentNumberOfPage + 1, ParallelOptions, pdfPageNumber =>
            {
                var pdfDocument = pdfDocumentObjectPool.Get();
                var pdfPage = pdfDocument.GetPage(pdfPageNumber);
                var pdfImages = pdfPage.GetImages();
                var imageResponses = new ConcurrentBag<ImageResponse>();
                foreach (var pdfImage in pdfImages)
                {
                    var imageBytes = GetImageBytes(pdfImage);
                    if (imageBytes.Length == 0) continue;
                    var preparateImageBytes = PreparateImage(imageBytes);
                    var engine = tesseractEngineObjectPool.Get();
                    try
                    {
                        using var imageDocument = Pix.LoadFromMemory(preparateImageBytes);
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
                pdfDocumentObjectPool.Return(pdfDocument);
            });
            
            await context.Response.WriteAsJsonAsync(new Response
            {
                Pages = pageResponses.OrderBy(static response => response.Number),
            });
        });

        await app.RunAsync();
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
        using var image = new MagickImage();
        try
        {
            image.Read(bytes);
            image.AutoOrient();
            image.AutoLevel();
            image.Despeckle();
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