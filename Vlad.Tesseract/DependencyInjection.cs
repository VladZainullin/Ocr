using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;

namespace Vlad.Tesseract;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddTesseract(this IHostApplicationBuilder builder)
    {
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