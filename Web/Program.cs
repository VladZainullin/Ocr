using System.Net.Mime;
using Domain;
using ImageService;
using OcrService;
using Serilog;
using Web.Services;

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
            builder.AddOcr();
            
            builder
                .AddWeb()
                .AddImageService();

            await using var app = builder.Build();

            app.UseForwardedHeaders();
            
            app.UseSerilogRequestLogging();

            app.UseResponseCompression();
            
            app.UseHttpsRedirection();
            app.UseHsts();

            app.MapPost("api/v3/documents", static async context =>
            {
                if (context.Request.Form.Files.Count < 1
                    || context.Request.Form.Files[0].ContentType != MediaTypeNames.Application.Pdf
                    || context.Request.Form.Files[0].Length == 0)
                {
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Pages = Array.Empty<PageModel>(),
                    });
                    return;
                }

                var parallelOptions = new ParallelOptions
                {
                    CancellationToken = context.RequestAborted,
                    MaxDegreeOfParallelism = Math.Min(Math.Max(1, Environment.ProcessorCount - 1), 16),
                };

                var pdfService = context.RequestServices.GetRequiredService<PdfService>();

                await using var stream = context.Request.Form.Files[0].OpenReadStream();
                var response = pdfService.Process(stream, parallelOptions);

                await context.Response.WriteAsJsonAsync(response);
            });

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