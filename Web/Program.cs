using Application;
using Carter;
using ImageService;
using OcrService;
using Serilog;

namespace Web;

file sealed class Program
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

            app.UseResponseCompression();
            
            app.UseHttpsRedirection();
            app.UseHsts();

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