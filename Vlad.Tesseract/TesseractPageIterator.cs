using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

public class TesseractPageIterator : ITesseractPageIterator
{
    private readonly bool _ownsHandle;

    public TesseractPageIterator(nint handle, bool ownsHandle)
    {
        if (handle == 0) throw new ArgumentException("Native iterator handle cannot be null.", nameof(handle));
        
        _ownsHandle = ownsHandle;
        Handle = handle;
    }

    public nint Handle { get; private set; }

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

    public IPix GetBinaryImage(PageIteratorLevel level)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pixPtr = TesseractNative.TessPageIteratorGetBinaryImage(Handle, level);
        return pixPtr == 0 
            ? throw new InvalidOperationException("TessPageIteratorGetBinaryImage returned a null pointer.") 
            : new Pix(pixPtr);
    }

    public IPix GetImage(PageIteratorLevel level, int padding, IPix originalImage, out int left, out int top)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        ArgumentOutOfRangeException.ThrowIfNegative(padding);
        ArgumentNullException.ThrowIfNull(originalImage);
        var pixPtr =
            TesseractNative.TessPageIteratorGetImage(Handle, level, padding, originalImage.Handle, out left, out top);
        if (pixPtr == 0)
        {
            throw new InvalidOperationException(
                "TessPageIteratorGetImage returned a null pointer.");
        }
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
        return TesseractNative.TessPageIteratorIsAtFinalElement(Handle, level, element);
    }

    public bool TryGetBaseline(PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBaseline(Handle, level,
            out x1, out y1, out x2, out y2);
    }

    public bool TryGetBoundingBox(PageIteratorLevel level, out int left, out int top, out int right, out int bottom)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBoundingBox(Handle, level, out left, out top, out right, out bottom);
    }

    public PolygonBlockType GetBlockType()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        return TesseractNative.TessPageIteratorBlockType(Handle);
    }

    public virtual ITesseractPageIterator Copy()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        var pageIteratorPtr = TesseractNative.TessPageIteratorCopy(Handle);
        return pageIteratorPtr == 0 
            ? throw new InvalidOperationException("TessPageIteratorCopy returned a null pointer.") 
            : new TesseractPageIterator(pageIteratorPtr, true);
    }

    protected virtual void ReleaseHandle(nint handle)
    {
        TesseractNative.TessPageIteratorDelete(handle);
    }

    public void Dispose()
    {
        if (Disposed)
            return;

        Disposed = true;

        var handle = Handle;
        Handle = 0;

        if (_ownsHandle && handle != 0)
        {
            ReleaseHandle(handle);
        }

        GC.SuppressFinalize(this);
    }
}