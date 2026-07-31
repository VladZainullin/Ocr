using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

public sealed class TesseractMonitor : IDisposable
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool CancelCallback(
        nint cancelThis,
        int words);
    
    private readonly nint _handle = TesseractNative.TessMonitorCreate();
    private bool _disposed;
    
    private CancelCallback? _cancelCallback;

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
                // Исключение нельзя передавать из C# в native-код.
                // При ошибке останавливаем распознавание.
                return true;
            }
        };
        
        var callbackPointer =
            Marshal.GetFunctionPointerForDelegate(_cancelCallback);

        TesseractNative.TessMonitorSetCancelFunc(
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

        TesseractNative.TessMonitorDelete(_handle);

        _cancelCallback = null;
    }
}