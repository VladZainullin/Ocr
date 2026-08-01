using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using OcrService.Contracts;
using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddTesseract(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<ITesseractNativeLogBridge>(static sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            
            if (OperatingSystem.IsLinux())
            {
                return new LinuxTesseractNativeLogBridge(loggerFactory);
            }

            if (OperatingSystem.IsMacOS())
            {
                return new MacOsTesseractNativeLogBridge(loggerFactory);
            }

            if (OperatingSystem.IsWindows())
            {
                return new WindowsTesseractNativeLogBridge(loggerFactory);
            }
            
            throw new NotSupportedException("Tesseract logger not supported");
        });
        
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