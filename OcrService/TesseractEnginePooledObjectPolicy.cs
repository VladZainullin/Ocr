using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Tesseract;

namespace OcrService;

internal sealed class TesseractEnginePooledObjectPolicy(IOptions<TesseractOptions> options)
    : PooledObjectPolicy<TesseractEngine>
{
    public override TesseractEngine Create()
    {
        return new TesseractEngine(options.Value.Path, options.Value.Language);
    }

    public override bool Return(TesseractEngine obj)
    {
        return !obj.IsDisposed;
    }
}