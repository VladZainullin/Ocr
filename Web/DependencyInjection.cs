using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Serilog;
using Tesseract;
using Web.Services;

namespace Web;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddWeb(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog();
        
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

        builder.WebHost.ConfigureKestrel(static options => options.Limits.MaxRequestBodySize = 100 * 1024 * 1024);

        builder.Services.TryAddSingleton<ImageService>();
        
        builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        builder.Services.TryAddSingleton<ObjectPool<TesseractEngine>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = new TesseractEnginePooledObjectPolicy("./tesseract", "rus+eng");
            return provider.Create(policy);
        });
        
        builder.Services.TryAddSingleton<OcrService>();
        builder.Services.TryAddSingleton<PdfService>();
        
        return builder;
    }
}