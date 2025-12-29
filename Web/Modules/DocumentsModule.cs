using System.Net.Mime;
using Application.Contracts;
using Carter;
using Domain;

namespace Web.Modules;

public sealed class DocumentsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
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

            var pdfService = context.RequestServices.GetRequiredService<IPdfService>();

            await using var stream = context.Request.Form.Files[0].OpenReadStream();
            var response = await pdfService.ProcessAsync(stream, context.RequestAborted);
                
            await context.Response.WriteAsJsonAsync(response);
        });
    }
}