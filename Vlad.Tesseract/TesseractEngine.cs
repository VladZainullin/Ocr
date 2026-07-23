using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

internal sealed class TesseractEngine : IDisposable
{
    private readonly IntPtr _handle;
    private bool _disposed;

    public TesseractEngine()
    {
        _handle = TesseractNative.TessBaseApiCreate();
    }

    public void SetVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetVariable(_handle, name, value);
    }

    public bool TryGetVariable(string name, out string? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (TesseractNative.TessBaseApiGetStringVariable(_handle, name, out var v))
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }

    public void SetSegmentationMode(PageSegmentationMode mode)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetPageSegMode(_handle, mode);
    }

    public bool TryInitialization(string dataPath, string language, TessOcrEngineMode oem)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiInit2(_handle, dataPath, language, oem) == 0;
    }

    public void SetImage(Pix image)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetImage2(_handle, image.Handle);
    }

    public unsafe void SetImage(byte[] imageData, uint width, uint height, uint bytesPerPixel)
    {
        var bytesPerLine = width * bytesPerPixel;
        fixed (byte* imagePtr = imageData)
        {
            TesseractNative.TessBaseApiSetImage(_handle, (nint)imagePtr, width, height, bytesPerPixel, bytesPerLine);
        }
    }

    public string Recognize()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var textPtr = TesseractNative.TessBaseAPIGetUTF8Text(_handle);
        try
        {
            return Marshal.PtrToStringUTF8(textPtr) ?? string.Empty;
        }
        finally
        {
            TesseractNative.TessDeleteText(textPtr);
        }
    }

    public TesseractIterator GetIterator()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        var iterator = TesseractNative.TessBaseApiGetIterator(_handle);
        return new TesseractIterator(iterator);
    }

    public void Clear()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiClear(_handle);
    }

    public void Dispose()
    {
        if (_disposed) return;
        TesseractNative.TessBaseApiDelete(_handle);
        _disposed = true;
    }
}