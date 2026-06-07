using System.Text;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.FeatureManagement;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

namespace Web;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddWeb(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            builder.Configuration.GetSection("Kestrel").Bind(options);
        });

        builder.Services.AddProblemDetails();

        builder.Services
            .AddOpenTelemetry()
            .WithTracing(static tracerProviderBuilder =>
            {
                tracerProviderBuilder.AddAspNetCoreInstrumentation();
                tracerProviderBuilder.AddHttpClientInstrumentation();
                tracerProviderBuilder.AddOtlpExporter();
            })
            .WithMetrics(static meterProviderBuilder =>
            {
                meterProviderBuilder.AddRuntimeInstrumentation();
                meterProviderBuilder.AddAspNetCoreInstrumentation();
                meterProviderBuilder.AddHttpClientInstrumentation();
                meterProviderBuilder.AddOtlpExporter();
            });

        builder.Services.AddFeatureManagement();
        
        builder.Services.AddSerilog();
        
        builder.Services.TryAddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider
        {
            MaximumRetained = Environment.ProcessorCount,
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        
        builder.Services.TryAddSingleton<StringBuilderPooledObjectPolicy>();
        builder.Services.TryAddSingleton<ObjectPool<StringBuilder>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = serviceProvider.GetRequiredService<StringBuilderPooledObjectPolicy>();
            return provider.Create(policy);
        });

        builder.Services.AddAuthorization();
        
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtBearerOptions =>
            {
                builder.Configuration.GetSection("Sso").Bind(jwtBearerOptions);
            });

        builder.Services.AddCarter();
        
        builder.Services.AddHsts(options =>
        {
            builder.Configuration.GetSection("Hsts").Bind(options);
        });

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {   
            builder.Configuration.GetSection("ForwardedHeaders").Bind(options);
        });

        builder.Services.AddCors(options =>
        {
            var cors = builder.Configuration.GetSection("Cors");
            
            options.AddDefaultPolicy(policy =>
            {
                var origins = cors.GetSection("Origins").Get<string[]>();
                if (!ReferenceEquals(origins, null))
                {
                    policy.WithOrigins(origins);
                }
                else
                {
                    policy.AllowAnyOrigin();
                }
                
                var methods = cors.GetSection("Methods").Get<string[]>();
                if (!ReferenceEquals(methods, null))
                {
                    policy.WithMethods(methods);
                }
                else
                {
                    policy.AllowAnyMethod();
                }
                
                var headers = cors.GetSection("Headers").Get<string[]>();
                if (!ReferenceEquals(headers, null))
                {
                    policy.WithHeaders(headers);
                }
                else
                {
                    policy.AllowAnyHeader();
                }
            });
        });

        builder.Services.AddResponseCompression(static options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes;

            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        });

        builder.Services.AddHealthChecks();

        builder.Host.UseDefaultServiceProvider(options =>
        {
            builder.Configuration.GetSection("ServiceProvider").Bind(options);
        });
        
        return builder;
    }
}