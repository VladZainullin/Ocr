using Microsoft.Extensions.ObjectPool;
using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

internal sealed class TesseractEnginePooledObjectPolicy(ITesseractNativeLogBridge logBridge) : IPooledObjectPolicy<TesseractEngine>
{
    private readonly ITesseractNativeLogBridge _logBridge = logBridge;

    public TesseractEngine Create()
    {
        var engine = new TesseractEngine();
        
        if (!engine.TryInitialization("/Users/vadislavzainullin/MEGAsync/tesseract", "rus+eng",
                TessOcrEngineMode.OemLstmOnly)) 
            throw new InvalidOperationException("Cannot create Tesseract engine");

        engine.SetSegmentationMode(PageSegmentationMode.Auto);
            
        return engine;

    }

    public bool Return(TesseractEngine engine)
    {
        engine.Clear();
        return true;
    }
}