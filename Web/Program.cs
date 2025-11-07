using System.Buffers;
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

            var pageResponses = ArrayPool<PageResponse>.Shared.Rent(pdfDocument.NumberOfPages);

            var pdfPages = pdfDocument.GetPages();

            foreach (var pdfPage in pdfPages)
            {
                var pdfImages = pdfPage.GetImages();
                var imageResponses = new LinkedList<string>();
                foreach (var pdfImage in pdfImages)
                {
                    var imageBytes = GetImageBytes(pdfImage);
                    if (imageBytes.Length == 0) continue;
                    var preparateImageBytes = PreparateImage(imageBytes);
                    var engine = tesseractEngineObjectPool.Get();
                    if (preparateImageBytes.Length == 0) continue;
                    try
                    {
                        using var imageDocument = Pix.LoadFromMemory(preparateImageBytes);
                        using var imagePage = engine.Process(imageDocument);
                        var text = imagePage.GetText();
                        imageResponses.AddLast(text);
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

            var pageResponses = new LinkedList<PageResponse>();

            var pdfPages = pdfDocument.GetPages();

            foreach (var pdfPagesInChunk in pdfPages.Chunk(Environment.ProcessorCount))
            {
                var chunk = new LinkedList<Buffer>();
                
                foreach (var pdfPage in pdfPagesInChunk)
                {
                    var pdfImages = pdfPage.GetImages();
                    var memories = new LinkedList<byte[]>();
                    foreach (var pdfImage in pdfImages)
                    {
                        var imageBytes = GetImageBytes(pdfImage);
                        memories.AddLast(imageBytes);
                    }

                    chunk.AddLast(new Buffer
                    {
                        Number = pdfPage.Number,
                        Text = pdfPage.Text,
                        Images = memories
                    });
                }
                
                Parallel.ForEach(
                    chunk,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount,
                    },
                    buffer =>
                    {
                        var imageResponses = new LinkedList<string>();
                        foreach (var imageMemory in buffer.Images)
                        {
                            if (imageMemory.Length == 0) continue;
                            var preparateImageBytes = PreparateImage(imageMemory);
                            if (preparateImageBytes.Length == 0) return;
                            var engine = tesseractEngineObjectPool.Get();
                            try
                            {
                                using var imageDocument = Pix.LoadFromMemory(preparateImageBytes);
                                using var imagePage = engine.Process(imageDocument);
                                var text = imagePage.GetText();
                                imageResponses.AddLast(text);
                            }
                            finally
                            {
                                tesseractEngineObjectPool.Return(engine);
                            }
                        }
                        
                        pageResponses.AddLast(new PageResponse
                        {
                            Number = buffer.Number,
                            Text = buffer.Text,
                            Images = imageResponses
                        });
                    });
            }

            await context.Response.WriteAsJsonAsync(new Response
            {
                Pages = pageResponses,
            });
        });

        await app.RunAsync();
    }

    private static byte[] GetImageBytes(IPdfImage pdfImage)
    {
        if (pdfImage.TryGetPng(out var pngImageBytes))
        {
            return pngImageBytes;
        }

        if (pdfImage.TryGetBytesAsMemory(out var memory))
        {
            return memory.ToArray();
        }

        return pdfImage.RawBytes.ToArray();
    }

    private static byte[] PreparateImage(Span<byte> bytes)
    {
        using var image = new MagickImage();
        try
        {
            image.Read(bytes);
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

public sealed class Buffer
{
    public required int Number { get; set; }
    
    public required string Text { get; set; }

    public required LinkedList<byte[]> Images { get; set; }
}

public sealed class Response
{
    public required IEnumerable<PageResponse> Pages { get; init; }
}

public sealed class PageResponse
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public required IEnumerable<string> Images { get; init; }
}