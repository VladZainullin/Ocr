using System;
using Microsoft.Extensions.ObjectPool;

namespace Vlad.Tesseract;

internal sealed class TesseractEnginePooledObjectPolicy : IPooledObjectPolicy<TesseractEngine>
{
    public TesseractEngine Create()
    {
        var engine = new TesseractEngine();

        engine.SetVariable("debug_file", "/dev/stderr");
        
        if (!engine.TryInitialization("/Users/vadislavzainullin/MEGAsync/tesseract", "rus+eng",
                TessOcrEngineMode.OemLstmOnly)) 
            throw new InvalidOperationException("Cannot create Tesseract engine");
        
        var a = engine.GetVariable("debug_file");

        engine.SetSegmentationMode(PageSegmentationMode.Auto);
            
        return engine;

    }

    public bool Return(TesseractEngine engine)
    {
        engine.Clear();
        return true;
    }
}