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

    public static string Version => TesseractNative.TessVersion();

    public static string DataPath => TesseractNative.TessBaseApiGetDataPath();

    public PageSegmentationMode PageSegmentationMode
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiGetPageSegMode(_handle);
        }
    }

    public void SetVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetVariable(_handle, name, value);
    }

    public void SetDebugVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetDebugVariable(_handle, name, value);
    }

    public void SetInputName(Pix pix)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetInputName(_handle, pix.Handle);
    }

    public string? GetVariable(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var pointer = TesseractNative.TessBaseApiGetStringVariable(
            _handle,
            name);

        return Marshal.PtrToStringUTF8(pointer);
    }
    
    public bool TryGetVariable(string name, out int? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (TesseractNative.TessBaseApiGetIntVariable(_handle, name, out var v))
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }
    
    public bool TryGetVariable(string name, out double? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (TesseractNative.TessBaseApiGetDoubleVariable(_handle, name, out var v))
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }
    
    public bool TryGetVariable(string name, out bool? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (TesseractNative.TessBaseApiGetBoolVariable(_handle, name, out var v))
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }

    public void SetInputName(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetInputName(_handle, name);
    }

    public string InputName
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiGetInputName(_handle);
        }
    }

    public string Text
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseAPIGetUTF8Text(_handle);
        }
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

    public void SetRectangle(int left, int top, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetRectangle(left, top, width, height);
    }

    public unsafe void SetImage(byte[] imageData, uint width, uint height, uint bytesPerPixel)
    {
        var bytesPerLine = width * bytesPerPixel;
        fixed (byte* imagePtr = imageData)
        {
            TesseractNative.TessBaseApiSetImage(_handle, (nint)imagePtr, width, height, bytesPerPixel, bytesPerLine);
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