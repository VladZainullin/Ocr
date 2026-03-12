using ImageService.Contracts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ImageService;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddImageService(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<IImageService, ImageService>();
            
        return builder;
    }
}