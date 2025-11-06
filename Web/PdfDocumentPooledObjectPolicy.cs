using Microsoft.Extensions.ObjectPool;
using UglyToad.PdfPig;

namespace Web;

internal sealed class PdfDocumentPooledObjectPolicy(Func<PdfDocument> create) : PooledObjectPolicy<PdfDocument>
{
    public override PdfDocument Create()
    {
        return create();
    }

    public override bool Return(PdfDocument obj)
    {
        return true;
    }
}