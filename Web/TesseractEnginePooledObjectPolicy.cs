using Microsoft.Extensions.ObjectPool;
using Tesseract;
using UglyToad.PdfPig;

namespace Web;

internal sealed class TesseractEnginePooledObjectPolicy(string path, string language) : PooledObjectPolicy<TesseractEngine>
{
    public override TesseractEngine Create()
    {
        return new TesseractEngine(path, language);
    }

    public override bool Return(TesseractEngine obj)
    {
        return !obj.IsDisposed;
    }
}

internal sealed class PdfDocumentPooledObjectPolicy(byte[] bytes) : PooledObjectPolicy<PdfDocument>
{
    public override PdfDocument Create()
    {
        return PdfDocument.Open(bytes);
    }

    public override bool Return(PdfDocument obj)
    {
        return true;
    }
}