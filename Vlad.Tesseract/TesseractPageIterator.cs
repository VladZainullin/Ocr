using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

public class TesseractPageIterator(nint handle) : ITesseractPageIterator
{
    public nint Handle { get; set; } = handle;

    protected bool Disposed { get; private set; }

    public void Begin()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        TesseractNative.TessPageIteratorBegin(Handle);
    }

    public void GetOrientation(out OrientationPage orientation, out WritingDirection writingDirection,
        out TextLineOrder textLineOrder, out float deskewAngle)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        TesseractNative.TessPageIteratorOrientation(Handle, out orientation, out writingDirection, out textLineOrder
            , out deskewAngle);
    }

    public IPix GetImage(PageIteratorLevel level, int padding, IPix originalImage, out int left, out int top)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pixPtr =
            TesseractNative.TessPageIteratorGetImage(Handle, level, padding, originalImage.Handle, out left, out top);
        return new Pix(pixPtr);
    }

    public void GetParagraphInfo(
        out ParagraphJustification justification, out bool isListItem, out bool isCrown, out int firstLineIndent)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        TesseractNative.TessPageIteratorParagraphInfo(Handle, out justification, out isListItem, out isCrown,
            out firstLineIndent);
    }

    public virtual bool TryNext(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorNext(Handle, level);
    }

    public bool IsAtBeginningOf(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorIsAtBeginningOf(Handle, level);
    }

    public bool IsAtFinalElement(PageIteratorLevel level, PageIteratorLevel element)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorIsAtFinalElement(Handle, level, element) != 0;
    }

    public bool TryGetBaseLine(PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBaseline(Handle, level,
            out x1, out y1, out x2, out y2);
    }

    public bool TryGetBoundingBox(PageIteratorLevel level, out int x, out int y, out int width, out int height)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBoundingBox(Handle, level, out x, out y, out width, out height);
    }

    public PolygonBlockType GetBlockType()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBlockType(Handle);
    }

    public virtual ITesseractPageIterator Copy()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pageIteratorPage = TesseractNative.TessPageIteratorCopy(Handle);
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

        if (Handle != 0)
        {
            TesseractNative.TessPageIteratorDelete(Handle);
            Handle = 0;
        }

        Disposed = true;
    }
}