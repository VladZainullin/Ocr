using System.Runtime.InteropServices;
using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

public sealed class TesseractResultIterator(nint handle)
    : TesseractPageIterator(handle, true), ITesseractResultIterator
{
    public override ITesseractResultIterator Copy()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pointer = TesseractNative.TessResultIteratorCopy(Handle);
        return pointer == 0
            ? throw new InvalidOperationException("TessResultIteratorCopy returned a null pointer.")
            : new TesseractResultIterator(pointer);
    }

    public ITesseractPageIterator GetPageIterator()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pageIteratorPtr = TesseractNative.TessResultIteratorGetPageIterator(Handle);
        return pageIteratorPtr == 0
            ? throw new InvalidOperationException("TessResultIteratorGetPageIterator returned a null pointer.")
            : new TesseractPageIterator(pageIteratorPtr, false);
    }

    public ITesseractPageIterator GetPageIteratorConst()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);

        var pointer = TesseractNative.TessResultIteratorGetPageIteratorConst(Handle);
        return pointer == 0
            ? throw new InvalidOperationException("TessResultIteratorGetPageIteratorConst returned a null pointer.")
            : new TesseractPageIterator(pointer, false);
    }

    public ITesseractChoiceIterator GetChoiceIterator()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);

        var pointer = TesseractNative.TessResultIteratorGetChoiceIterator(Handle);
        return pointer == 0
            ? throw new InvalidOperationException("TessResultIteratorGetChoiceIterator returned a null pointer.")
            : new TesseractChoiceIterator(pointer);
    }

    public override bool TryNext(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorNext(Handle, level);
    }

    public string? GetText(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pointer = TesseractNative.TessResultIteratorGetUtf8Text(Handle, level);
        if (pointer == 0)
            return null;
        try
        {
            return Marshal.PtrToStringUTF8(pointer);
        }
        finally
        {
            TesseractNative.TessDeleteText(pointer);
        }
    }

    public float GetConfidence(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorConfidence(Handle, level);
    }

    public string? WordRecognitionLanguage()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordRecognitionLanguage(Handle);
    }

    public string? GetWordFontAttributes(out bool isBold, out bool isItalic, out bool isUnderlined, out bool isMonospace,
        out bool isSerif, out bool isSmallCaps, out int pointSize, out int fontId)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordFontAttributes(
            Handle, out isBold, out isItalic, out isUnderlined, out isMonospace, out isSerif, out isSmallCaps,
            out pointSize, out fontId);
    }

    public bool IsWordFromDictionary()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordIsFromDictionary(Handle);
    }

    public bool IsWordNumeric()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorWordIsNumeric(Handle);
    }

    public bool IsSymbolSuperscript()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsSuperscript(Handle);
    }

    public bool IsSymbolSubscript()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsSubscript(Handle);
    }

    public bool IsSymbolDropCap()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessResultIteratorSymbolIsDropcap(Handle);
    }
    
    protected override void ReleaseHandle(nint handle)
    {
        TesseractNative.TessResultIteratorDelete(handle);
    }
}