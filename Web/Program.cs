using System.Text;
using Tesseract;
using UglyToad.PdfPig;

namespace Web;

file sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton(static sp =>
        {
            var applicationLifetime = sp.GetRequiredService<IHostApplicationLifetime>();

            var engine = new TesseractEngine("./tesseract", "rus");
            applicationLifetime.ApplicationStopping.Register(() => engine.Dispose());

            return engine;
        });

        await using var app = builder.Build();

        app.MapPost("/documents", static async context =>
        {
            var engine = context.RequestServices.GetRequiredService<TesseractEngine>();
            await using var file = context.Request.Form.Files[0].OpenReadStream();
            using var document = PdfDocument.Open(file);
            var pages = document.GetPages();

            var builder = new StringBuilder();
            
            foreach (var page in pages)
            {
                var searchableText = page.Text;
                builder.AppendLine(searchableText);
                builder.AppendLine();
                
                var images = page.GetImages();
                foreach (var image in images)
                {
                    if (image.TryGetPng(out var pngBytes))
                    {
                        using var imageDocument = Pix.LoadFromMemory(pngBytes);
                        using var imagePage = engine.Process(imageDocument);
                        var text = imagePage.GetText();
                        builder.AppendLine(text);
                        builder.AppendLine();
                    }
                    else
                    {
                        using var imageDocument = Pix.LoadFromMemory(image.RawBytes.ToArray());
                        using var imagePage = engine.Process(imageDocument);
                        var text = imagePage.GetText();
                        builder.AppendLine(text);
                        builder.AppendLine();
                    }
                }
            }
            
            var result = builder.ToString();
            await context.Response.WriteAsJsonAsync(result);
        });

        await app.RunAsync();
    }
}