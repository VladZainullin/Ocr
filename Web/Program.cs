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

            var pdfPages = pdfDocument.GetPages();

            var pageResponses = new PageResponse[pdfDocument.NumberOfPages];
            foreach (var pdfPage in pdfPages)
            {
                var pdfImages = pdfPage.GetImages();
                var imageResponses = new List<string>();
                foreach (var pdfImage in pdfImages)
                {
                    var imageBytes = GetImageBytes(pdfImage);
                    if (imageBytes.Length == 0) continue;

                    var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    tempFilePath = Path.ChangeExtension(tempFilePath, ".tmp");

                    await using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
                    try
                    {
                        await fileStream.WriteAsync(imageBytes);
                        imageResponses.Add(tempFilePath);
                    }
                    catch
                    {
                        fileStream.Close();
                        File.Delete(tempFilePath);
                        throw;
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }

                pageResponses[pdfPage.Number - 1] = new PageResponse
                {
                    Number = pdfPage.Number,
                    Text = pdfPage.Text,
                    Images = imageResponses
                };
            }

            await Parallel.ForEachAsync(pageResponses, new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, async (page, _) =>
            {
                var imageResponses = new List<string>();
                foreach (var image in page.Images)
                {
                    await using var fileStream = new FileStream(image, FileMode.Open, FileAccess.Read);
                    try
                    {
                        var preparateImageBytes = PreparateImage(fileStream);
                        var engine = tesseractEngineObjectPool.Get();
                        if (preparateImageBytes.Length == 0) continue;
                        try
                        {
                            using var imageDocument = Pix.LoadFromMemory(preparateImageBytes);
                            using var imagePage = engine.Process(imageDocument);
                            var text = imagePage.GetText();
                            imageResponses.Add(text);
                        }
                        finally
                        {
                            tesseractEngineObjectPool.Return(engine);
                        }
                    }
                    finally
                    {
                        fileStream.Close();
                        File.Delete(image);
                    }
                }

                page.Images = imageResponses;
            });

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
                        var preparateImageBytes = PreparateImage(imageBytes.Span);
                        var engine = tesseractEngineObjectPool.Get();
                        if (preparateImageBytes.Length == 0) continue;
                        try
                        {
                            using var imageDocument = Pix.LoadFromMemory(preparateImageBytes);
                            using var imagePage = engine.Process(imageDocument);
                            var text = imagePage.GetText();
                            imageResponses.Add(text);
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

    private static Memory<byte> GetImageBytes(IPdfImage pdfImage)
    {
        if (pdfImage.TryGetPng(out var pngImageBytes))
        {
            return pngImageBytes;
        }

        if (pdfImage.TryGetBytesAsMemory(out var memory))
        {
            return memory;
        }

        return pdfImage.RawMemory;
    }

    private static byte[] PreparateImage(Span<byte> bytes)
    {
        try
        {
            using var image = new MagickImage(bytes);
            image.Grayscale();
            image.Strip();
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

public sealed class Buffer
{
    public required int Number { get; set; }

    public required string Text { get; set; }

    public required List<Memory<byte>> Images { get; set; }
}

public sealed class Response
{
    public required IReadOnlyCollection<PageResponse> Pages { get; init; }
}

public sealed class PageBuffer
{
    public required int Number { get; set; }

    public required string Text { get; init; }

    public required string Path { get; init; }
}

public sealed class PageResponse
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public required IReadOnlyCollection<string> Images { get; set; }
}