using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

public sealed class TesseractResultResultRenderer(nint handle) : ITesseractResultRenderer, IDisposable
{
    private bool _disposed;

    public string Extension
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessResultRendererExtention(handle);
        }
    }

    public string Title
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessResultRendererTitle(handle);
        }
    }

    public int ImageNumbers
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessResultRendererImageNum(handle);
        }
    }

    public bool TryNext()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessResultRendererNext(handle);
    }

    public void Insert(ITesseractResultRenderer renderer)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public bool TryBeginDocument(string title)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessResultRendererBeginDocument(handle, title);
    }

    public bool TryAddImage(ITesseractEngine engine)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessResultRendererAddImage(handle, engine.Handle);
    }

    public bool TryEndDocument()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessResultRendererEndDocument(handle);
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        TesseractNative.TessDeleteResultRenderer(handle);
    }
}