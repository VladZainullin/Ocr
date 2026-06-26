using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

public unsafe class TesseractLibrary : IDisposable
{
    private readonly nint _libraryHandle;
    private readonly delegate* unmanaged[Cdecl]<nint> _tessVersion;
    private bool _disposed;

    public TesseractLibrary(string dllPath)
    {
        if (!File.Exists(dllPath)) throw new FileNotFoundException($"Library not found: {dllPath}");

        _libraryHandle = NativeLibrary.Load(dllPath);
        if (_libraryHandle == nint.Zero)
            throw new DllNotFoundException($"Failed to load library: {dllPath}");

        var versionPtr = NativeLibrary.GetExport(_libraryHandle, "TessVersion");

        if (versionPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessVersion not found in the library");
        
        _tessVersion = (delegate* unmanaged[Cdecl]<nint>)versionPtr;
    }

    public nint GetVersion() => _tessVersion();

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_libraryHandle != nint.Zero)
            {
                NativeLibrary.Free(_libraryHandle);
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}