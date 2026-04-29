using Application;
using Carter;
using ImageService;
using Microsoft.FeatureManagement;
using OcrService;
using Serilog;
using Web.Features.ResponseCompression;

namespace Web;

file static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        try
        {
            builder
                .AddOcr()
                .AddImageService()
                .AddApplication();
            
            builder
                .AddWeb();

            await using var app = builder.Build();
            
            app.UseForwardedHeaders();
            
            app.UseSerilogRequestLogging();
            
            app.Use(async (context, next) =>
            {
                var featureManager = context.RequestServices.GetRequiredService<IFeatureManager>();

                var compressionEnabled = await featureManager.IsEnabledAsync("ResponseCompression");
                context.Features.Set<IResponseCompressionFeature>(new HttpsCompressionFeature
                {
                    Enable = compressionEnabled,
                });

                await next(context);
            });
            
            app.UseWhen(
                static context =>
                {
                    var enable = context.Features.Get<IResponseCompressionFeature>()?.Enable;
                    return enable ?? false;
                }, 
                static app => app.UseResponseCompression());
            
            app.UseHttpsRedirection();
            app.UseHsts();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapCarter();

            app.MapHealthChecks("/health");

            await app.RunAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}