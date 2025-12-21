using ImageService.Contracts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ImageService;

public static class DependencyInjection
{
    extension(IHostApplicationBuilder builder) 
    {
        public IHostApplicationBuilder AddImageService()
        {
            builder.Services.TryAddSingleton<IImageService, ImageService>();
            
            return builder;
        }
    }
}