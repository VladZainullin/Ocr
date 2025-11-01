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
            using var engine = new TesseractEngine("./tesseract", "rus");
            await using var file = context.Request.Form.Files[0].OpenReadStream();
            using var document = PdfDocument.Open(file);
            var pages = document.GetPages();
            
            var pageResponses = ProcessPages(pages, engine);
            
            await context.Response.WriteAsJsonAsync(new Response
            {
                Pages = pageResponses,
            });
        });

        await app.RunAsync();
    }

    private static IEnumerable<PageResponse> ProcessPages(IEnumerable<Page> pages, TesseractEngine engine)
    {
        return pages.Select(page => ProcessPage(engine, page));
    }

    private static PageResponse ProcessPage(TesseractEngine engine, Page page)
    {
        var searchableText = page.Text;

        var imageResponses = ProcessImages(page, engine);

        var hyperlinks = ProcessHyperlinks(page);
        
        var annotationResponse =  ProcessAnnotations(page);

        return new PageResponse
        {
            Number = page.Number,
            Text = searchableText,
            Images = imageResponses,
            Hyperlinks = hyperlinks,
            Annotations = annotationResponse
        };

    }

    private static IEnumerable<string> ProcessHyperlinks(Page page)
    {
        var hyperlinks = page.GetHyperlinks();
        foreach (var hyperlink in hyperlinks)
        {
            yield return hyperlink.Text;
        }
    }

    private static IEnumerable<AnnotationResponse> ProcessAnnotations(Page page)
    {
        var annotations= page.GetAnnotations();

        foreach (var annotation in annotations)
        {
            yield return new AnnotationResponse
            {
                Name = annotation.Name,
                Text = annotation.Content
            };
        }
    }

    private static IEnumerable<ImageResponse> ProcessImages(Page page, TesseractEngine engine)
    {
        var images = page.GetImages();
        foreach (var image in images)
        {
            if (image.TryGetPng(out var pngBytes))
            {
                using var imageDocument = Pix.LoadFromMemory(pngBytes);
                using var imagePage = engine.Process(imageDocument);
                var text = imagePage.GetText();
                yield return new ImageResponse
                {
                    Text = text
                };

            }
            else
            {
                var bytes = image.RawBytes.ToArray();
                using var imageDocument = Pix.LoadFromMemory(bytes);
                using var imagePage = engine.Process(imageDocument);
                var text = imagePage.GetText();
                yield return new ImageResponse
                {
                    Text = text
                };
            }
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

    public required IEnumerable<string> Hyperlinks { get; init; }

    public required IEnumerable<AnnotationResponse> Annotations { get; init; }
}

public sealed class ImageResponse
{
    public required string Text { get; init; }
}

public sealed class AnnotationResponse
{
    public required string? Name { get; init; }
    
    public required string? Text { get; init; }
}