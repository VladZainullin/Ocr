using System.Text;
using Carter;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Serilog;

namespace Web;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddWeb(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog();
        
        builder.Services.TryAddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider
        {
            MaximumRetained = Environment.ProcessorCount,
        });
        builder.Services.TryAddSingleton<ObjectPool<StringBuilder>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = serviceProvider.GetRequiredService<StringBuilderPooledObjectPolicy>();
            return provider.Create(policy);
        });

        builder.Services.AddCarter();
        
        builder.Services.AddHsts(static options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        builder.Services.Configure<ForwardedHeadersOptions>(static options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;
        });

        builder.Services.AddCors(static options =>
        {
            options.AddDefaultPolicy(static policy =>
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        builder.Services.AddResponseCompression(static options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes;

            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        });

        builder.Services.AddHealthChecks();

        builder.Host.UseDefaultServiceProvider(static options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

        builder.WebHost.ConfigureKestrel(static options => options.Limits.MaxRequestBodySize = 100 * 1024 * 1024);
        
        return builder;
    }
}