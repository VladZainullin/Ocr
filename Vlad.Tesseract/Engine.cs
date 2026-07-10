using System.Runtime.InteropServices;
using System.Text;

namespace Vlad.Tesseract;

public sealed unsafe class Engine
{
    private const int MaxStackSize = 256;

    private readonly nint _enginePtr;
    private readonly nint _handle;

    private delegate* unmanaged[Cdecl]<nint, byte*, byte*, TessOcrEngineMode, byte**, int, int> _tessBaseApiInit1;
    private delegate* unmanaged[Cdecl]<nint, byte*, byte*, TessOcrEngineMode, int> _tessBaseApiInit2;
    private delegate* unmanaged[Cdecl]<nint, byte*, byte*, int> _tessBaseApiInit3;

    public Engine(nint enginePtr, nint handle)
    {
        _enginePtr = enginePtr;
        _handle = handle;
    }

    public bool TryInitialization(
        ReadOnlySpan<char> dataPath,
        ReadOnlySpan<char> language,
        TessOcrEngineMode oem,
        ReadOnlySpan<string> configs)
    {
        _tessBaseApiInit1 = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, TessOcrEngineMode, byte**, int, int>)
            NativeLibrary.GetExport(_handle, "TessBaseAPIInit1");

        // Подсчёт размера строк с учётом завершающего нуля
        var dataPathByteCount = checked(Encoding.UTF8.GetByteCount(dataPath) + 1);
        var languageByteCount = checked(Encoding.UTF8.GetByteCount(language) + 1);

        byte* pDataPath;
        byte* pLanguage;
        byte** pConfigs = null;
        var configsHandle = IntPtr.Zero;
        IntPtr[] configPtrs = [];

        var freeDataPath = false;
        var freeLanguage = false;
        var freeConfigs = false;

        if (dataPathByteCount <= MaxStackSize)
        {
            var stackBuffer = stackalloc byte[dataPathByteCount];
            pDataPath = stackBuffer;
        }
        else
        {
            pDataPath = (byte*)Marshal.AllocHGlobal(dataPathByteCount);
            freeDataPath = true;
        }

        if (languageByteCount <= MaxStackSize)
        {
            var stackBuffer = stackalloc byte[languageByteCount];
            pLanguage = stackBuffer;
        }
        else
        {
            pLanguage = (byte*)Marshal.AllocHGlobal(languageByteCount);
            freeLanguage = true;
        }

        var configsSize = configs.IsEmpty ? 0 : configs.Length;
        if (configsSize > 0)
        {
            configPtrs = new IntPtr[configsSize];

            for (var i = 0; i < configsSize; i++)
            {
                var configBytes = Encoding.UTF8.GetBytes(configs[i]);
                var configPtr = Marshal.AllocHGlobal(configBytes.Length + 1);

                Marshal.Copy(configBytes, 0, configPtr, configBytes.Length);
                Marshal.WriteByte(configPtr, configBytes.Length, 0);

                configPtrs[i] = configPtr;
            }

            configsHandle = Marshal.AllocHGlobal(IntPtr.Size * configsSize);
            Marshal.Copy(configPtrs, 0, configsHandle, configsSize);
            pConfigs = (byte**)configsHandle;
            freeConfigs = true;
        }

        try
        {
            var dataPathSpan = new Span<byte>(pDataPath, dataPathByteCount);
            var writtenData = Encoding.UTF8.GetBytes(dataPath, dataPathSpan);
            dataPathSpan[writtenData] = 0;

            var languageSpan = new Span<byte>(pLanguage, languageByteCount);
            var writtenLang = Encoding.UTF8.GetBytes(language, languageSpan);
            languageSpan[writtenLang] = 0;

            return _tessBaseApiInit1(
                _enginePtr,
                pDataPath,
                pLanguage,
                oem,
                pConfigs,
                configsSize) == 0;
        }
        finally
        {
            if (freeDataPath) Marshal.FreeHGlobal((nint)pDataPath);
            if (freeLanguage) Marshal.FreeHGlobal((nint)pLanguage);
            if (freeConfigs)
            {
                foreach (var ptr in configPtrs)
                {
                    if (ptr != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptr);
                }

                // Освобождаем массив указателей
                if (configsHandle != IntPtr.Zero)
                    Marshal.FreeHGlobal(configsHandle);
            }
        }
    }

    public bool TryInitialization(ReadOnlySpan<char> dataPath, ReadOnlySpan<char> language, TessOcrEngineMode oem)
    {
        _tessBaseApiInit2 = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, TessOcrEngineMode, int>)
            NativeLibrary.GetExport(_handle, "TessBaseAPIInit2");

        var dataPathByteCount = checked(Encoding.UTF8.GetByteCount(dataPath) + 1);
        var languageByteCount = checked(Encoding.UTF8.GetByteCount(language) + 1);

        byte* pDataPath;
        byte* pLanguage;

        var freeDataPath = false;
        var freeLanguage = false;

        if (languageByteCount <= MaxStackSize)
        {
            var stackBuffer = stackalloc byte[languageByteCount];
            pLanguage = stackBuffer;
        }
        else
        {
            pLanguage = (byte*)Marshal.AllocHGlobal(languageByteCount);
            freeLanguage = true;
        }

        if (dataPathByteCount <= MaxStackSize)
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

            return _tessBaseApiInit2(_enginePtr, pDataPath, pLanguage, oem) == 0;
        }
        finally
        {
            if (freeDataPath) Marshal.FreeHGlobal((nint)pDataPath);
            if (freeLanguage) Marshal.FreeHGlobal((nint)pLanguage);
        }
    }

    public bool TryInitialization(ReadOnlySpan<char> dataPath, ReadOnlySpan<char> language)
    {
        _tessBaseApiInit3 = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int>)
            NativeLibrary.GetExport(_handle, "TessBaseAPIInit3");

        var dataPathByteCount = checked(Encoding.UTF8.GetByteCount(dataPath) + 1);
        var languageByteCount = checked(Encoding.UTF8.GetByteCount(language) + 1);

        byte* pDataPath;
        byte* pLanguage;

        var freeDataPath = false;
        var freeLanguage = false;

        if (languageByteCount <= MaxStackSize)
        {
            var stackBuffer = stackalloc byte[languageByteCount];
            pLanguage = stackBuffer;
        }
        else
        {
            pLanguage = (byte*)Marshal.AllocHGlobal(languageByteCount);
            freeLanguage = true;
        }

        if (dataPathByteCount <= MaxStackSize)
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