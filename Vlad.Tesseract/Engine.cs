using System.Runtime.InteropServices;
using System.Text;

namespace Vlad.Tesseract;

public sealed unsafe class Engine
{
    private readonly nint _enginePtr;
    private readonly nint _handle;

    private delegate* unmanaged[Cdecl]<nint, byte*, byte*, int> _tessBaseApiInit3;

    public Engine(nint enginePtr, nint handle)
    {
        _enginePtr = enginePtr;
        _handle = handle;
    }
    
    public bool TryInitialization(ReadOnlySpan<char> dataPath, ReadOnlySpan<char> language)
    {
        _tessBaseApiInit3 = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int>)
            NativeLibrary.GetExport(_handle, "TessBaseAPIInit3");
        
        const int maxStackSize = 256;
        var dataPathByteCount = checked(Encoding.UTF8.GetByteCount(dataPath) + 1);
        var languageByteCount = checked(Encoding.UTF8.GetByteCount(language) + 1);

        byte* pDataPath;
        byte* pLanguage;

        var freeDataPath = false;
        var freeLanguage = false;

        if (languageByteCount <= maxStackSize)
        {
            var stackBuffer = stackalloc byte[languageByteCount];
            pLanguage = stackBuffer;
        }
        else
        {
            pLanguage = (byte*)Marshal.AllocHGlobal(languageByteCount);
            freeLanguage = true;
        }

        if (dataPathByteCount <= maxStackSize)
        {
            var stackBuffer = stackalloc byte[dataPathByteCount];
            pDataPath = stackBuffer;
        }
        else
        {
            pDataPath = (byte*)Marshal.AllocHGlobal(dataPathByteCount);
            freeDataPath = true;
        }

        try
        {
            var dataPathSpan = new Span<byte>(pDataPath, dataPathByteCount);
            var writtenData = Encoding.UTF8.GetBytes(dataPath, dataPathSpan);
            dataPathSpan[writtenData] = 0;

            var languageSpan = new Span<byte>(pLanguage, languageByteCount);
            var writtenLang = Encoding.UTF8.GetBytes(language, languageSpan);
            languageSpan[writtenLang] = 0;

            return _tessBaseApiInit3(_enginePtr, pDataPath, pLanguage) == 0;
        }
        finally
        {
            if (freeDataPath) Marshal.FreeHGlobal((nint)pDataPath);
            if (freeLanguage) Marshal.FreeHGlobal((nint)pLanguage);
        }
    }
}