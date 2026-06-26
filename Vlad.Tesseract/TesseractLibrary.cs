using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

public unsafe class TesseractLibrary : IDisposable
{
    private readonly nint _libraryHandle;
    private readonly delegate* unmanaged[Cdecl]<nint> _tessVersion;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiVersion;
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

        var baseApiVersionPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIVersion");
        if (baseApiVersionPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIVersion not found in the library");
        _tessBaseApiVersion = (delegate* unmanaged[Cdecl]<nint, nint>)baseApiVersionPtr;
    }

    /// <summary>
    /// Returns the version string of the Tesseract library.
    /// </summary>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the version. The caller must NOT free this pointer.</returns>
    public nint GetVersion() => _tessVersion();

    /// <summary>
    /// Returns the version string of the Tesseract library associated with the given BaseAPI handle.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the version.</returns>
    public nint TessBaseApiVersion(nint handle) => _tessBaseApiVersion(handle);

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