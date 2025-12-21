using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Tesseract;
using Web;

namespace Ocr;

public static class DependencyInjection
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddOcr()
        {
            builder.Services
                .AddOptions<TesseractOptions>()
                .BindConfiguration("Tesseract")
                .Validate(
                    static tesseractOptions => !string.IsNullOrWhiteSpace(tesseractOptions.Language), 
                    """Tesseract Language is required. Add "Tesseract__Language" to environment variables""")
                .Validate(
                    static tesseractOptions => !string.IsNullOrWhiteSpace(tesseractOptions.Language), 
                    """Tesseract Path is required. Add "Tesseract__Path" to environment variables""")
                .ValidateOnStart();
        
            builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            builder.Services.TryAddSingleton<ObjectPool<TesseractEngine>>(static serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = new TesseractEnginePooledObjectPolicy("/Users/vadislavzainullin/Downloads/tesseract", "rus+eng");
                return provider.Create(policy);
            });
        
            builder.Services.TryAddSingleton<OcrService>();
            
            return builder;
        }
    }
}