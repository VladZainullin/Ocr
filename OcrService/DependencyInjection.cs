using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using Tesseract;

namespace OcrService;

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
            builder.Services.TryAddSingleton<TesseractEnginePooledObjectPolicy>();
            builder.Services.TryAddSingleton<ObjectPool<TesseractEngine>>(static serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = serviceProvider.GetRequiredService<TesseractEnginePooledObjectPolicy>();
                return provider.Create(policy);
            });
        
            builder.Services.TryAddSingleton<IOcrService, OcrService>();
            
            return builder;
        }
    }
}