using System.Net.Mime;
using Application.Contracts;
using Carter;
using Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Web.Modules;

public sealed class DocumentsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/documents", HandleDocument).DisableAntiforgery();
    }

    private static async Task<Results<BadRequest<string>, Ok<ResponseModel>>> HandleDocument(
        IFormFileCollection files,
        IPdfService pdfService,
        CancellationToken cancellationToken)
    {
        if (files.Count != 1)
        {
            return TypedResults.BadRequest("Exactly one file is required");
        }

        var file = files[0];

        if (file.Length == 0 ||
            !file.ContentType.StartsWith(MediaTypeNames.Application.Pdf, StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("Invalid PDF file");
        }

        await using var stream = file.OpenReadStream();
        var response = await pdfService.ProcessAsync(stream, cancellationToken);

        return TypedResults.Ok(response);
    }
}