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

    public bool WordIsFromDictionary()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordIsFromDictionary(Iterator);
    }

    public bool WordIsNumeric()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordIsNumeric(Iterator);
    }

    public bool SymbolIsSuperscript()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsSuperscript(Iterator);
    }

    public bool SymbolIsSubscript()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsSubscript(Iterator);
    }

    public bool SymbolIsDropcast()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsDropcast(Iterator);
    }

    public ChoiceIterator GetChoiceIterator()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var choiceIteratorPtr = TesseractNative.TessResultIteratorGetChoiceIterator(Iterator);
        return new ChoiceIterator(choiceIteratorPtr);
    }

    protected override void Dispose(bool disposing)
    {
        if (Disposed) return;
        if (Iterator != nint.Zero)
        {
            TesseractNative.TessResultIteratorDelete(Iterator);
            Iterator = nint.Zero;
        }
        
        base.Dispose(disposing);
    }
}