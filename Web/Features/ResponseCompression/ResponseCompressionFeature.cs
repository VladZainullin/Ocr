namespace Web.Features.ResponseCompression;

internal sealed class HttpsCompressionFeature : IResponseCompressionFeature
{
    public bool Enable { get; init; }
}