using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

internal static class Native
{
    private const string DllName = @"C:\Users\user\RiderProjects\Ocr\Web\bin\Debug\net10.0\x64\tesseract50.dll";
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPICreate();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPIDelete(IntPtr handle);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPISetPageSegMode(IntPtr handle, PageSegMode mode);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false,
        ThrowOnUnmappableChar = true)]
    public static extern int TessBaseAPIInit3(IntPtr handle,
        [MarshalAs(UnmanagedType.LPStr)] string dataPath,
        [MarshalAs(UnmanagedType.LPStr)] string language);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text")]
    public static extern IntPtr ResultIteratorGetUTF8TextInternal(IntPtr handle, PageIteratorLevel level);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPISetImage(IntPtr handle, IntPtr imagedata, uint width, uint height,
        uint bytesPerPixel, uint bytesPerLine);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int TessBaseAPIRecognize(IntPtr handle, IntPtr monitor);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetUTF8Text(IntPtr handle);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessDeleteText(IntPtr text);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetIterator(IntPtr handle);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool TessPageIteratorNext(IntPtr iterator, PageIteratorLevel level);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
    public static extern int PageIteratorIsAtFinalElement(IntPtr handle, PageIteratorLevel level,
        PageIteratorLevel element);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
    public static extern void PageIteratorDelete(IntPtr handle);
}