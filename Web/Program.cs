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

        var annotationResponse = ProcessAnnotations(page);

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
        var annotations = page.GetAnnotations();

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
        var pdfImages = page.GetImages();
        foreach (var pdfImage in pdfImages)
        {
            if (pdfImage.TryGetPng(out var pngBytes))
            {
                using var imageDocument = Pix.LoadFromMemory(pngBytes);
                using var imagePage = engine.Process(imageDocument);
                var text = imagePage.GetText();
                yield return new ImageResponse
                {
                    Text = text
                };
            }
            else if (pdfImage.TryGetBytesAsMemory(out var memory))
            {
                var bytes = memory.ToArray();
                using var imageDocument = Pix.LoadFromMemory(bytes);
                using var imagePage = engine.Process(imageDocument);
                var text = imagePage.GetText();
                yield return new ImageResponse
                {
                    Text = text
                };
            }
            else
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
                var stream = new MemoryStream();
                image.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                
                using var imageDocument = Pix.LoadFromMemory(stream.ToArray());
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

public class FileFormatDetector
{
    private static readonly Dictionary<string, byte[]> Signatures = new()
    {
        { "PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
        { "JPEG", new byte[] { 0xFF, 0xD8, 0xFF } },
        { "GIF", new byte[] { 0x47, 0x49, 0x46 } },
        { "BMP", new byte[] { 0x42, 0x4D } },
        { "PDF", new byte[] { 0x25, 0x50, 0x44, 0x46 } },
        { "ZIP", new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
        { "DOCX", new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
        { "XLSX", new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
        { "PPTX", new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
        { "MP3", new byte[] { 0x49, 0x44, 0x33 } }, // ID3 tag
        { "MP4", new byte[] { 0x66, 0x74, 0x79, 0x70 } }, // ftyp
        { "EXE", new byte[] { 0x4D, 0x5A } }, // MZ
        { "DLL", new byte[] { 0x4D, 0x5A } }  // MZ
    };

    public static string Detect(byte[] bytes)
    {
        if (bytes.Length < 4)
            return "UNKNOWN";

        foreach (var signature in Signatures)
        {
            if (bytes.Length >= signature.Value.Length && 
                bytes.Take(signature.Value.Length).SequenceEqual(signature.Value))
            {
                return signature.Key;
            }
        }

        return "UNKNOWN";
    }
}