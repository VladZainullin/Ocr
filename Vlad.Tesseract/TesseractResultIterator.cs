namespace Vlad.Tesseract;

public sealed class TesseractResultIterator(nint iterator) : TesseractPageIterator(iterator)
{
    public string GetText(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorGetUtf8Text(Iterator, level);
    }
    
    public float GetConfidence(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorGetConfidence(Iterator, level);
    }
}