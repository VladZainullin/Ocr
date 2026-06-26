using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Vlad.Tesseract;

internal static partial class Native
{
    private const string DllName = @"C:\Users\user\RiderProjects\Ocr\Web\bin\Debug\net10.0\x64\tesseract50.dll";
    
    [LibraryImport(DllName, EntryPoint = "TessPageIteratorBegin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorBegin(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorBoundingBox")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorBoundingBox(IntPtr iterator, PageIteratorLevel level,
        out int left, out int top, out int right, out int bottom);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorBlockType")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial PolyBlockType TessPageIteratorBlockType(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorGetBinaryImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessPageIteratorGetBinaryImage(IntPtr iterator, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorGetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessPageIteratorGetImage(IntPtr iterator, PageIteratorLevel level, int padding,
        IntPtr originalImage, out int left, out int top);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorBaseline")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorBaseline(IntPtr iterator, PageIteratorLevel level, out int x1, 
        out int y1, out int x2, out int y2);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorOrientation")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorOrientation(
        IntPtr iterator,
        out OrientationPage orientation,
        out WritingDirection writingDirection,
        out TextlineOrder textlineOrder, 
        out float deskewAngle);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorParagraphInfo")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorParagraphInfo(
        IntPtr iterator,
        out ParagraphJustification justification,
        [MarshalAs(UnmanagedType.Bool)] out bool isListItem,
        [MarshalAs(UnmanagedType.Bool)] out bool isCrown,
        out int firstLineIndent);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorCopy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessPageIteratorCopy(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorGetWordFontAttributes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorGetWordFontAttributes(
        IntPtr iterator,
        [MarshalAs(UnmanagedType.Bool)] out bool isBold, 
        [MarshalAs(UnmanagedType.Bool)] out bool isItalic, 
        [MarshalAs(UnmanagedType.Bool)] out bool isUnderlined, 
        [MarshalAs(UnmanagedType.Bool)] out bool isMonospace, 
        [MarshalAs(UnmanagedType.Bool)] out bool isSerif,
        [MarshalAs(UnmanagedType.Bool)] out bool isSmallCaps, 
        out int pointSize,
        out int fontId);
    
    [LibraryImport(DllName, EntryPoint = "TessBaseAPICreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiCreate();

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiDelete(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetPageSegMode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetPageSegMode(IntPtr handle, PageSegmentMode mode);

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
    public static partial int TessPageIteratorIsAtFinalElement(IntPtr iterator, PageIteratorLevel level,
        PageIteratorLevel element);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorDelete(IntPtr iterator);
}