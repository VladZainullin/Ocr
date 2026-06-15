using System.Buffers;
using System.IO.Hashing;
using Application.Contracts;
using Domain.Models;
using Microsoft.Extensions.Caching.Hybrid;

namespace Application;

internal sealed class CachedPdfService(
    HybridCache cache,
    IPdfService pdfService) : IPdfService
{
    public async Task<ResponseModel> ProcessAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var hash = ComputeXxHash128FromStreamSpan(stream);
        var key = Convert.ToHexString(hash);
        var result = await cache.GetOrCreateAsync(
            key, 
            async ct => await pdfService.ProcessAsync(stream, ct), cancellationToken: cancellationToken);

        return result;
    }

    private static byte[] ComputeXxHash128FromStreamSpan(Stream stream)
    {
        var xxHash = new XxHash128();
        
        var buffer = ArrayPool<byte>.Shared.Rent(81920); // ~80 КБ
        try
        {
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                xxHash.Append(buffer.AsSpan(0, bytesRead));
            }
        
            return xxHash.GetHashAndReset();
        }
        finally
        {
            stream.Seek(0, SeekOrigin.Begin);
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}