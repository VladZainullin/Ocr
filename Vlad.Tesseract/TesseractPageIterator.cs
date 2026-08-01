using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

public class TesseractPageIterator(nint iterator) : IDisposable, ITesseractPageIterator
{
    public nint Iterator { get; set; } = iterator;

    protected bool Disposed { get; private set; }

    public void Begin()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        TesseractNative.TessPageIteratorBegin(Iterator);
    }

    public bool NextElement(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorNext(Iterator, level);
    }

    public bool IsAtBeginningOf(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorIsAtBeginningOf(Iterator, level);
    }

    public bool IsAtFinalElement(PageIteratorLevel level, PageIteratorLevel element)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorIsAtFinalElement(Iterator, level, element) != 0;
    }

    public bool TryGetBaseLine(PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBaseline(Iterator, level,
            out x1, out y1, out x2, out y2);
    }

    public bool TryGetBoundingBox(PageIteratorLevel level, out int x, out int y, out int width, out int height)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBoundingBox(Iterator, level, out x, out y, out width, out height);
    }

    public PolyBlockType GetBlockType()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBlockType(Iterator);
    }

    public virtual ITesseractPageIterator Copy()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pageIteratorPage = TesseractNative.TessPageIteratorCopy(Iterator);
        return new TesseractPageIterator(pageIteratorPage);
    }

    public void Dispose()
    {
        Dispose(Disposed);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (disposing) return;

        if (Iterator != 0)
        {
            TesseractNative.TessPageIteratorDelete(Iterator);
            Iterator = 0;
        }

        Disposed = true;
    }
}