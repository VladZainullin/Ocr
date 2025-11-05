using System.Collections.Concurrent;
using ImageMagick;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Tesseract;
using UglyToad.PdfPig;
using Page = UglyToad.PdfPig.Content.Page;

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

        app.MapPost("/documents", static async context =>
        {
            await using var file = context.Request.Form.Files[0].OpenReadStream();
            using var document = PdfDocument.Open(file);
            var pages = document.GetPages().ToArray();
            
            var tesseractEngineObjectPool = context.RequestServices.GetRequiredService<ObjectPool<TesseractEngine>>();

            var pageResponses = new ConcurrentBag<PageResponse>();
            
            Parallel.ForEach(pages, page =>
            {
                var searchableText = page.Text;

                var imageResponses = new ConcurrentBag<ImageResponse>();
                try
                {
                    var pdfImages = page.GetImages().ToArray();
                    Parallel.ForEach(pdfImages, pdfImage =>
                    {
                        using var image = new MagickImage();

                        try
                        {
                            image.Ping(pdfImage.RawBytes);
                        }
                        catch (MagickMissingDelegateErrorException)
                        {
                            return;
                        }

                        try
                        {
                            image.Read(pdfImage.RawBytes);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        image.AutoOrient();
                        image.Despeckle();
                        image.Grayscale();
                        var bytes = image.ToByteArray();
                    
                        using var imageDocument = Pix.LoadFromMemory(bytes);
                        var engine = tesseractEngineObjectPool.Get();
                        using var imagePage = engine.Process(imageDocument);
                        var text = imagePage.GetText();
                        imageResponses.Add(new ImageResponse
                        {
                            Text = text
                        });
                        tesseractEngineObjectPool.Return(engine);
                    });

                    pageResponses.Add(new PageResponse
                    {
                        Number = page.Number,
                        Text = searchableText,
                        Images = imageResponses,
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });

            await context.Response.WriteAsJsonAsync(new Response
            {
                Pages = pageResponses,
            });
        });

        await app.RunAsync();
    }

    private static IEnumerable<string> ProcessHyperlinks(Page page)
    {
        var hyperlinks = page.GetHyperlinks();
        foreach (var hyperlink in hyperlinks)
        {
            yield return hyperlink.Text;
        }
    }

    private static IEnumerable<ImageResponse> ProcessImages(Page page)
    {
        var pdfImages = page.GetImages();
        foreach (var pdfImage in pdfImages)
        {
            
            using var image = new MagickImage();

            try
            {
                image.Ping(pdfImage.RawBytes);
            }
            catch (MagickMissingDelegateErrorException)
            {
                continue;
            }
                
            image.Read(pdfImage.RawBytes);
            image.AutoOrient();
            image.Despeckle();
            image.Grayscale();
            var stream = new MemoryStream();
            image.Write(stream);
                
            using var imageDocument = Pix.LoadFromMemory(stream.ToArray());
            using var engine = new TesseractEngine("./tesseract", "rus");
            using var imagePage = engine.Process(imageDocument);
            var text = imagePage.GetText();
            yield return new ImageResponse
            {
                Text = text
            };
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