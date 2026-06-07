using Application;
using Carter;
using ImageService;
using Microsoft.FeatureManagement;
using OcrService;
using Serilog;

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
            
            var featureManager = app.Services.GetRequiredService<IFeatureManager>();
            
            if (await featureManager.IsEnabledAsync("ForwardedHeaders"))
            {
                app.UseForwardedHeaders();
            }

            app.UseCors();
            
            app.UseExceptionHandler();
            app.UseStatusCodePages();
            
            app.UseSerilogRequestLogging();

            if (await featureManager.IsEnabledAsync("ResponseCompression"))
            {
                app.UseResponseCompression();
            }
            
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