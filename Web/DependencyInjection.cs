using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Web.Services;

namespace Web;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddWeb(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog();

        builder.Services.AddHealthChecks();
        
        builder.Host.UseDefaultServiceProvider(static options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

        builder.WebHost.ConfigureKestrel(static options => options.Limits.MaxRequestBodySize = 100 * 1024 * 1024);
        
        builder.Services.TryAddSingleton<PdfService>();
        
        return builder;
    }
}