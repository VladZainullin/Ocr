namespace Vlad.Tesseract;

public sealed class ChoiceIterator(nint iterator) : IDisposable
{
    private bool _disposed;
    
    public bool Next()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessChoiceIteratorNext(iterator);
    }

    public string GetText()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessChoiceIteratorGetUtf8Text(iterator);
    }

    public float GetConfidence()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessChoiceIteratorConfidence(iterator);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        TesseractNative.TessChoiceIteratorDelete(iterator);
    }
}