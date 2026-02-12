using System.Net.Mime;
using Carter;
using Domain;
using ImageService.Contracts;
using OcrService.Contracts;

namespace Web.Modules;

public sealed class ImagesModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/images", static async context =>
        {
            if (context.Request.Form.Files.Count < 1
                || context.Request.Form.Files[0].ContentType != MediaTypeNames.Image.Jpeg
                || context.Request.Form.Files[0].Length == 0)
            {
                await context.Response.WriteAsJsonAsync(new ImageModel
                {
                    Blocks = []
                });
                return;
            }

            var imageService = context.RequestServices.GetRequiredService<IImageService>();
            var ocrService = context.RequestServices.GetRequiredService<IOcrService>();

            await using var stream = context.Request.Form.Files[0].OpenReadStream();
            var bytes = imageService.Recognition(stream);
            var blocks = ocrService.Process(bytes);

            await context.Response.WriteAsJsonAsync(new ImageModel
            {
                Blocks = blocks
            });
        });
    }
}