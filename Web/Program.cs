using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Tesseract;
using UglyToad.PdfPig;

namespace Web;

file sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        builder.Services.TryAddSingleton<ObjectPool<StringBuilder>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = new StringBuilderPooledObjectPolicy();
            return provider.Create(policy);
        });
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

            var stringBuilderObjectPool = context.RequestServices.GetRequiredService<ObjectPool<StringBuilder>>();
            var builder = stringBuilderObjectPool.Get();
            
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
            stringBuilderObjectPool.Return(builder);
            await context.Response.WriteAsJsonAsync(result);
        });

        await app.RunAsync();
    }
}