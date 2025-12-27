using Domain;

namespace Application.Contracts;

public interface IPdfService
{
    Task<ResponseModel> ProcessAsync(Stream stream, CancellationToken cancellationToken = default);
}