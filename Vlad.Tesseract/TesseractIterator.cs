using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

public sealed class TesseractIterator(nint iterator) : IDisposable
{
    private bool _disposed;
    
    public void Begin()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessPageIteratorBegin(iterator);
    }
    
    public string? GetText(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var textPtr = TesseractNative.TessResultIteratorGetUtf8Text(iterator, level);
        try
        {
            var text = Marshal.PtrToStringUTF8(textPtr);
            return text;
        }
        finally
        {
            TesseractNative.TessDeleteText(textPtr);
        }
    }

    public bool Next(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessPageIteratorNext(iterator, level);
    }

    public bool IsAtFinalElement(PageIteratorLevel level, PageIteratorLevel element)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessPageIteratorIsAtFinalElement(iterator, level, element) != 0;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        TesseractNative.TessResultIteratorDelete(iterator);
    }
}