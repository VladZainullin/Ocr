using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ImageService;

public static class DependencyInjection
{
    extension(IHostApplicationBuilder builder) 
    {
        public IHostApplicationBuilder AddImageService()
        {
            builder.Services.TryAddSingleton<ImageService>();
            
            return builder;
        }
    }
}