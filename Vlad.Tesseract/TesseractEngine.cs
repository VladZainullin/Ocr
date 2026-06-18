using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

public sealed class TesseractEngine : IDisposable
{
    private readonly IntPtr _handle;
    private bool _disposed;

    public TesseractEngine(string dataPath, string language)
    {
        _handle = Native.TessBaseAPICreate();
        if (_handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to create Vlad.Tesseract API instance.");

        if (Native.TessBaseAPIInit3(_handle, dataPath, language) != 0)
        {
            Dispose();
            throw new InvalidOperationException($"Failed to initialize Vlad.Tesseract with language '{language}'.");
        }
    }

    public string Recognize(byte[] imageData, uint width, uint height, uint bytesPerPixel)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var handle = GCHandle.Alloc(imageData, GCHandleType.Pinned);
        try
        {
            var bytesPerLine = width * bytesPerPixel;
            Native.TessBaseAPISetImage(_handle, handle.AddrOfPinnedObject(), width, height, bytesPerPixel, bytesPerLine);

            var textPtr = Native.TessBaseAPIGetUTF8Text(_handle);
            if (textPtr == IntPtr.Zero)
                return string.Empty;

            try
            {
                return Marshal.PtrToStringUTF8(textPtr) ?? string.Empty;
            }
            finally
            {
                Native.TessDeleteText(textPtr);
            }
        }
        finally
        {
            handle.Free();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        Native.TessBaseAPIDelete(_handle);
        _disposed = true;
    }
}