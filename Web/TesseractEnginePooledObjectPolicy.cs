using Microsoft.Extensions.ObjectPool;
using Tesseract;

namespace Web;

internal sealed class TesseractEnginePooledObjectPolicy(string path, string language)
    : PooledObjectPolicy<TesseractEngine>
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