using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

public sealed class TesseractResultIterator(nint handle) : TesseractPageIterator(handle), ITesseractResultIterator
{
    public override ITesseractResultIterator Copy()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var resultIterator = TesseractNative.TessResultIteratorCopy(Handle);
        return new TesseractResultIterator(resultIterator);
    }

    public ITesseractPageIterator GetPageIterator()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pageIteratorPtr = TesseractNative.TessResultIteratorGetPageIterator(Handle);
        return new TesseractPageIterator(pageIteratorPtr);
    }

    public ITesseractPageIterator GetPageIteratorConst()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pageIteratorPtr = TesseractNative.TessResultIteratorGetPageIteratorConst(Handle);
        return new TesseractPageIterator(pageIteratorPtr);
    }

    public ITesseractChoiceIterator GetChoiceIterator()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var choiceIteratorPtr = TesseractNative.TessResultIteratorGetChoiceIterator(Handle);
        return new TesseractChoiceIterator(choiceIteratorPtr);
    }

    public override bool TryNext(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorNext(Handle, level);
    }

    public string GetText(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorGetUtf8Text(Handle, level);
    }

    public float GetConfidence(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorConfidence(Handle, level);
    }

    public string WordRecognitionLanguage()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordRecognitionLanguage(Handle);
    }

    public string GetWordFontAttributes(out bool isBold, out bool isItalic, out bool isUnderlined, out bool isMonospace,
        out bool isSerif, out bool isSmallCaps, out int pointSize, out int fontId)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordFontAttributes(
            Handle, out isBold, out isItalic, out isUnderlined, out isMonospace, out isSerif, out isSmallCaps,
            out pointSize, out fontId);
    }

    public bool WordIsFromDictionary()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordIsFromDictionary(Handle);
    }

    public bool WordIsNumeric()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordIsNumeric(Handle);
    }

    public bool SymbolIsSuperscript()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsSuperscript(Handle);
    }

    public bool SymbolIsSubscript()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsSubscript(Handle);
    }

    public bool SymbolIsDropCap()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsDropcap(Handle);
    }

    public override void Dispose(bool disposing)
    {
        if (Disposed) return;
        if (Handle != nint.Zero)
        {
            TesseractNative.TessResultIteratorDelete(Handle);
            Handle = nint.Zero;
        }

        base.Dispose(disposing);
    }
}