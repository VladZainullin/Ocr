using Microsoft.Extensions.ObjectPool;

namespace Vlad.Tesseract;

internal sealed class TesseractEnginePooledObjectPolicy : IPooledObjectPolicy<TesseractEngine>
{
    public TesseractEngine Create()
    {
        var engine = new TesseractEngine();

        if (!engine.TryInitialization("/Users/vadislavzainullin/MEGAsync/tesseract", "rus+eng",
                TessOcrEngineMode.OemLstmOnly)) 
            throw new InvalidOperationException("Cannot create Tesseract engine");
        
        engine.SetVariable("debug_file", "/dev/stderr");
        
        engine.TryGetVariable("log_level", out var logLevel);

        engine.SetSegmentationMode(PageSegmentationMode.Auto);
            
        return engine;

    }

    public bool Return(TesseractEngine engine)
    {
        engine.Clear();
        return true;
    }
}