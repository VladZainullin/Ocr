namespace Vlad.Tesseract;

internal sealed unsafe class Pix : IDisposable
{
    private nint _handle;
    private bool _disposed;
    
    public Pix(ReadOnlySpan<byte> data)
    {
        fixed (byte* ptr = data)
        {
            _handle = LeptonicaNative.PixReadMem((nint)ptr, data.Length);
        }
    }
    
    public nint Handle => _handle;

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        LeptonicaNative.PixDestroy(ref _handle);
    }
}