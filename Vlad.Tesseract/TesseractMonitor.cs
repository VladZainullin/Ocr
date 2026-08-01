using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

public sealed class TesseractMonitor : IDisposable
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool CancelCallback(
        nint cancelThis,
        int words);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool ProgressCallback(
        nint monitor,
        int left,
        int right,
        int top,
        int bottom);
    
    private readonly nint _handle = TesseractNative.TessMonitorCreate();
    private bool _disposed;
    
    private CancelCallback? _cancelCallback;
    private ProgressCallback? _progressCallback;

    public nint Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _handle;
        }
    }

    public void SetCancel(Func<int, bool> callback)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(callback);
        
        _cancelCallback = (_, words) =>
        {
            try
            {
                return callback(words);
            }
            catch
            {
                return true;
            }
        };
        
        var callbackPointer =
            Marshal.GetFunctionPointerForDelegate(_cancelCallback);

        TesseractNative.TessMonitorSetCancelFunc(
            _handle,
            callbackPointer);
    }
    
    public void SetProgress(
        Func<int, int, int, int, int, bool> callback)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(callback);

        _progressCallback = (
            monitor,
            left,
            right,
            top,
            bottom) =>
        {
            try
            {
                var progress =
                    TesseractNative.TessMonitorGetProgress(monitor);

                return callback(
                    progress,
                    left,
                    right,
                    top,
                    bottom);
            }
            catch
            {
                return false;
            }
        };

        var callbackPointer =
            Marshal.GetFunctionPointerForDelegate(_progressCallback);

        TesseractNative.TessMonitorSetProgressFunc(
            _handle,
            callbackPointer);
    }

    
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        TesseractNative.TessMonitorSetCancelFunc(
            _handle,
            nint.Zero);
        TesseractNative.TessMonitorSetProgressFunc(_handle, nint.Zero);

        TesseractNative.TessMonitorDelete(_handle);

        _cancelCallback = null;
    }
}