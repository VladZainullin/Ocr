using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

internal static partial class TesseractNative
{
    private const string LibraryName = "/opt/homebrew/lib/libtesseract.5.dylib";

    [LibraryImport(LibraryName, EntryPoint = "TessVersion", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessVersion();

    [LibraryImport(LibraryName, EntryPoint = "TessDeleteText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteText(nint text);

    [LibraryImport(LibraryName, EntryPoint = "TessDeleteTextArray")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteTextArray(nint arr);

    [LibraryImport(LibraryName, EntryPoint = "TessDeleteIntArray")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteIntArray(nint arr);

    [LibraryImport(LibraryName, EntryPoint = "TessTextRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessTextRendererCreate(string outputBase);

    [LibraryImport(LibraryName, EntryPoint = "TessHOcrRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessHOcrRendererCreate(string outputBase);

    [LibraryImport(LibraryName, EntryPoint = "TessHOcrRendererCreate2", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessHOcrRendererCreate2(string outputBase,
        [MarshalAs(UnmanagedType.Bool)] bool fontInfo);

    [LibraryImport(LibraryName, EntryPoint = "TessAltoRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessAltoRendererCreate(string outputBase);

    [LibraryImport(LibraryName, EntryPoint = "TessTsvRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessTsvRendererCreate(string outputBase);

    [LibraryImport(LibraryName, EntryPoint = "TessPDFRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessPdfRendererCreate(string outputBase, string dataDir,
        [MarshalAs(UnmanagedType.Bool)] bool textOnly);

    [LibraryImport(LibraryName, EntryPoint = "TessUnlvRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessUnlvRendererCreate(string outputBase);

    [LibraryImport(LibraryName, EntryPoint = "TessBoxTextRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBoxTextRendererCreate(string outputBase);

    [LibraryImport(LibraryName, EntryPoint = "TessLSTMBoxRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessLstmBoxRendererCreate(string outputBase);

    [LibraryImport(LibraryName, EntryPoint = "TessWordStrBoxRendererCreate",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessWordStrBoxRendererCreate(string outputBase);

    [LibraryImport(LibraryName, EntryPoint = "TessDeleteResultRenderer")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteResultRenderer(nint renderer);

    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererInsert")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessResultRendererInsert(nint renderer, nint subRenderer);

    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultRendererNext(nint renderer);

    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererBeginDocument",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultRendererBeginDocument(nint renderer, string title);

    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererAddImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultRendererAddImage(nint renderer, nint api);

    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererEndDocument")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultRendererEndDocument(nint renderer);

    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererExtention", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessResultRendererExtention(nint renderer);

    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererTitle", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessResultRendererTitle(nint renderer);

    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererImageNum")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessResultRendererImageNum(nint renderer);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPICreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiCreate();

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiDelete(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetOpenCLDevice", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetOpenClDevice(nint handle, out nint device);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetInputName", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetInputName(nint handle, string name);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetInputName", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetInputName(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetInputImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetInputImage(nint handle, nint pix);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetInputImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetInputImage(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetSourceYResolution")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiGetSourceYResolution(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetDatapath", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetDataPath();

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetOutputName", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetOutputName(nint handle, string name);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiSetVariable(nint handle, string name, string value);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetDebugVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiSetDebugVariable(nint handle, string name, string value);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetIntVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiGetIntVariable(nint handle, string name, out int value);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetBoolVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiGetBoolVariable(nint handle, string name,
        [MarshalAs(UnmanagedType.Bool)] out bool value);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetDoubleVariable",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiGetDoubleVariable(nint handle, string name, out double value);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetStringVariable",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetStringVariable(nint handle, string name);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIPrintVariables")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiPrintVariables(nint handle, nint fp);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIPrintVariablesToFile",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiPrintVariablesToFile(nint handle, string filename);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInit1", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int TessBaseApiInit1(nint handle, string? dataPath, string? language, OcrEngineMode oem,
        nint configs,
        int configsSize);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInit2", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiInit2(nint handle, string dataPath, string language, OcrEngineMode oem);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInit3", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiInit3(nint handle, string dataPath, string language);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInit4", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInit4(nint handle, string dataPath, string language, int mode,
        nint configs, int configsSize, nint varsVec, nint varsValues, nint varsVecSize,
        [MarshalAs(UnmanagedType.Bool)] bool setOnlyNonDebugParams);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetAvailableLanguagesAsVector",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetInitLanguagesAsString(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetAvailableLanguagesAsVector")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetAvailableLanguagesAsVector(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetLoadedLanguagesAsVector")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetLoadedLanguagesAsVector(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInitLangMod", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInitLangMod(nint handle, string? dataPath, string? language);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInitForAnalysePage",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiInitForAnalysePage(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIReadConfigFile", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiReadConfigFile(nint handle, string filename);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIReadDebugConfigFile",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiReadDebugConfigFile(nint handle, string filename);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetPageSegMode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetPageSegMode(nint handle, PageSegmentationMode mode);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetPageSegMode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial PageSegmentationMode TessBaseApiGetPageSegMode(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIRect", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiRect(nint handle, string imageData, int bytesPerPixel, int bytesPerLine,
        int left, int top, int width, int height);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIClearAdaptiveClassifier")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiClearAdaptiveClassifier(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetImage(nint handle, nint imageData, uint width, uint height,
        uint bytesPerPixel, uint bytesPerLine);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetImage2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetImage2(nint handle, nint pix);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetSourceResolution")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetSourceResolution(nint handle, int ppi);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetRectangle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetRectangle(int left, int top, int width, int height);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetThresholdedImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetThresholdedImage(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetRegions")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetRegions(nint handle, out nint pixa);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetTextlines")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetTextLines(nint handle, out nint pixa, out nint blockIds);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetTextlines1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetTextLines1(nint handle, [MarshalAs(UnmanagedType.Bool)] bool rawImage,
        int rawPadding, out nint pixa, out nint blockIds, out nint paraIds);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetStrips")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetStrips(nint handle, out nint pixa, out nint blockIds);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetWords")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetWords(nint handle, out nint pixa);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetConnectedComponents")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetConnectedComponents(nint handle, out nint connectedComponents);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetComponentImages")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetComponentImages(nint handle, PageIteratorLevel level,
        [MarshalAs(UnmanagedType.Bool)] bool textOnly, out nint pixa, out nint blockIds);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetComponentImages1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetComponentImages1(nint handle, PageIteratorLevel level,
        [MarshalAs(UnmanagedType.Bool)] bool textOnly, [MarshalAs(UnmanagedType.Bool)] bool rawImage,
        int rawPadding, out nint pixa, out nint blockIds, out nint paraIds);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetThresholdedImageScaleFactor")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiGetThresholdedImageScaleFactor(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIAnalyseLayout")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiAnalyseLayout(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIRecognize")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiRecognize(nint handle, nint monitor);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIRecognizeForChopTest")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiRecognizeForChopTest(nint handle, nint monitor);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIProcessPages", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiProcessPages(nint handle, string filename, string retryConfig,
        int timeoutMillis, nint renderer);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIProcessPage", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiProcessPage(nint handle, nint pix, int pageIndex, string? fileName,
        string? retryConfig, int timeoutMilliseconds, nint renderer);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetIterator(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetMutableIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetMutableIterator(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetUTF8Text", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetUtf8Text(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetHOCRText", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetHOcrText(nint handle, int pageNumber);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetAltoText", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetAltoText(nint handle, int pageNumber);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetTsvText", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetTsvText(nint handle, int pageNumber);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetBoxText", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetBoxText(nint handle, int pageNumber);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetLSTMBoxText", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetLstmBoxText(nint handle, int pageNumber);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetWordStrBoxText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetWordStrBoxText(nint handle, int pageNumber);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetUNLVText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetUnlvText(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIMeanTextConf")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiMeanTextConf(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIAllWordConfidences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiAllWordConfidences(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIAdaptToWordStr", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiAdaptToWordStr(nint handle, PageSegmentationMode mode, string wordStr);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIClear")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiClear(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIEnd")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiEnd(nint handle);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIIsValidWord", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiIsValidWord(nint handle, string word);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetTextDirection")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessBaseApiGetTextDirection(nint handle, out int offset, out float slope);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetUnichar", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessBaseApiGetUniChar(nint handle, int uniCharId);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetMinOrientationMargin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetMinOrientationMargin(nint handle, double margin);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorDelete(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorCopy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessPageIteratorCopy(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorBegin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorBegin(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessPageIteratorNext(nint iterator, PageIteratorLevel level);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessPageIteratorIsAtBeginningOf(nint iterator, PageIteratorLevel level);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorIsAtFinalElement")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessPageIteratorIsAtFinalElement(nint iterator, PageIteratorLevel level,
        PageIteratorLevel element);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorBoundingBox")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessPageIteratorBoundingBox(nint iterator, PageIteratorLevel level,
        out int left, out int top, out int right, out int bottom);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorBlockType")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial PolygonBlockType TessPageIteratorBlockType(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorGetBinaryImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessPageIteratorGetBinaryImage(nint iterator, PageIteratorLevel level);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorGetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessPageIteratorGetImage(nint iterator, PageIteratorLevel level, int padding,
        nint originalImagePtr, out int left, out int top);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorBaseline")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessPageIteratorBaseline(nint iterator, PageIteratorLevel level, out int x1,
        out int y1, out int x2, out int y2);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorOrientation")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorOrientation(
        nint iterator,
        out OrientationPage orientation,
        out WritingDirection writingDirection,
        out TextLineOrder textLineOrder,
        out float deskewAngle);

    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorParagraphInfo")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorParagraphInfo(
        nint iterator,
        out ParagraphJustification justification,
        [MarshalAs(UnmanagedType.Bool)] out bool isListItem,
        [MarshalAs(UnmanagedType.Bool)] out bool isCrown,
        out int firstLineIndent);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessResultIteratorDelete(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorCopy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorCopy(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetPageIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetPageIterator(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetPageIteratorConst")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetPageIteratorConst(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetChoiceIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetChoiceIterator(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultIteratorNext(nint iterator, PageIteratorLevel level);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetUTF8Text",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessResultIteratorGetUtf8Text(nint iterator, PageIteratorLevel level);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorConfidence")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float TessResultIteratorConfidence(nint iterator, PageIteratorLevel level);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordRecognitionLanguage",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessResultIteratorWordRecognitionLanguage(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordFontAttributes",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessResultIteratorWordFontAttributes(
        nint iterator,
        [MarshalAs(UnmanagedType.Bool)] out bool isBold,
        [MarshalAs(UnmanagedType.Bool)] out bool isItalic,
        [MarshalAs(UnmanagedType.Bool)] out bool isUnderlined,
        [MarshalAs(UnmanagedType.Bool)] out bool isMonospace,
        [MarshalAs(UnmanagedType.Bool)] out bool isSerif,
        [MarshalAs(UnmanagedType.Bool)] out bool isSmallCaps,
        out int pointSize,
        out int fontId);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordIsFromDictionary")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultIteratorWordIsFromDictionary(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordIsNumeric")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultIteratorWordIsNumeric(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorSymbolIsSuperscript")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultIteratorSymbolIsSuperscript(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorSymbolIsSubscript")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultIteratorSymbolIsSubscript(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorSymbolIsDropcap")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessResultIteratorSymbolIsDropcap(nint iterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessChoiceIteratorDelete(nint choiceIterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(TesseractBoolMarshaller))]
    public static partial bool TessChoiceIteratorNext(nint choiceIterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorGetUTF8Text",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial string TessChoiceIteratorGetUtf8Text(nint choiceIterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorConfidence")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float TessChoiceIteratorConfidence(nint choiceIterator);
    
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorCreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessMonitorCreate();

    [LibraryImport(LibraryName, EntryPoint = "TessMonitorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorDelete(nint monitor);

    [LibraryImport(LibraryName, EntryPoint = "TessMonitorSetCancelFunc")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetCancelFunc(nint monitor, nint cancelFunc);

    [LibraryImport(LibraryName, EntryPoint = "TessMonitorGetCancelThis")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessMonitorGetCancelThis(nint monitor);

    [LibraryImport(LibraryName, EntryPoint = "TessMonitorSetCancelThis")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetCancelThis(nint monitor, nint cancelThis);

    [LibraryImport(LibraryName, EntryPoint = "TessMonitorSetProgressFunc")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetProgressFunc(nint monitor, nint progress);
    
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorGetProgress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessMonitorGetProgress(nint monitor);

    [LibraryImport(LibraryName, EntryPoint = "TessMonitorSetDeadlineMSecs")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetDeadlineMSecs(nint monitor, int deadline);

    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIAdaptiveClassifier",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiAdaptiveClassifier(nint handle);
}