using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Vlad.Tesseract;

internal static partial class Native
{
    private const string DllName = @"C:\Users\user\RiderProjects\Ocr\Web\bin\Debug\net10.0\x64\tesseract50.dll";

    [LibraryImport(DllName, EntryPoint = "TessBaseAPICreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiCreate();

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiDelete(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetPageSegMode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetPageSegMode(IntPtr handle, PageSegMode mode);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIInit3", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInit3(IntPtr handle, string dataPath, string language);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetUTF8Text")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetUtf8Text(IntPtr handle, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetImage(IntPtr handle, IntPtr imagedata, uint width, uint height,
        uint bytesPerPixel, uint bytesPerLine);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIRecognize")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiRecognize(IntPtr handle, IntPtr monitor);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetUTF8Text")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseAPIGetUTF8Text(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessDeleteText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteText(IntPtr text);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetIterator(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorNext(IntPtr iterator, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorIsAtFinalElement")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessPageIteratorIsAtFinalElement(IntPtr handle, PageIteratorLevel level,
        PageIteratorLevel element);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorDelete(IntPtr handle);
}