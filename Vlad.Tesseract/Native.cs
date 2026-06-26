using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Vlad.Tesseract;

internal static partial class Native
{
    private const string DllName = @"C:\Users\user\RiderProjects\Ocr\Web\bin\Debug\net10.0\x64\tesseract50.dll";

    #region Version

    [LibraryImport(DllName, EntryPoint = "TessVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessVersion();

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiVersion(IntPtr handle);

    #endregion

    #region BaseAPI Lifecycle

    [LibraryImport(DllName, EntryPoint = "TessBaseAPICreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiCreate();

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiDelete(IntPtr handle);

    #endregion

    #region BaseAPI Initialization

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIInit3", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInit3(IntPtr handle, string dataPath, string language);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIInit4", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInit4(IntPtr handle, string dataPath, string language, int mode,
        IntPtr configs, int configsSize, IntPtr varsVec, IntPtr varsValues, IntPtr varsVecSize,
        [MarshalAs(UnmanagedType.Bool)] bool setOnlyNonDebugParams);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIInit5", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInit5(IntPtr handle, string dataPath, string language, int mode,
        IntPtr configs, int configsSize, IntPtr varsVec, IntPtr varsValues, IntPtr varsVecSize,
        [MarshalAs(UnmanagedType.Bool)] bool setOnlyNonDebugParams);

    #endregion

    #region BaseAPI Configuration

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiSetVariable(IntPtr handle, string name, string value);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetDebugVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiSetDebugVariable(IntPtr handle, string name, string value);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetIntVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiGetIntVariable(IntPtr handle, string name, out int value);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetBoolVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiGetBoolVariable(IntPtr handle, string name,
        [MarshalAs(UnmanagedType.Bool)] out bool value);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetDoubleVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiGetDoubleVariable(IntPtr handle, string name, out double value);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetStringVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetStringVariable(IntPtr handle, string name);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetOpenCLDevice", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetOpenClDevice(IntPtr handle, out IntPtr device);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIReadConfigFile", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiReadConfigFile(IntPtr handle, string filename);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIReadDebugConfigFile", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiReadDebugConfigFile(IntPtr handle, string filename);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetWarningHandler")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetWarningHandler(IntPtr handle, IntPtr warningHandler);

    #endregion

    #region BaseAPI Debug

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIPrintVariables")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiPrintVariables(IntPtr handle, IntPtr fp);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIPrintVariablesToFile", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiPrintVariablesToFile(IntPtr handle, string filename);

    #endregion

    #region BaseAPI Page Segmentation

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetPageSegMode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetPageSegMode(IntPtr handle, PageSegmentMode mode);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetPageSegMode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial PageSegmentMode TessBaseApiGetPageSegMode(IntPtr handle);

    #endregion

    #region BaseAPI Input/Output Names

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetInputName", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetInputName(IntPtr handle, string name);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetInputName")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetInputName(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetOutputName", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetOutputName(IntPtr handle, string name);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetOutputName")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetOutputName(IntPtr handle);

    #endregion

    #region BaseAPI Source Resolution

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetSourceResolution")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetSourceResolution(IntPtr handle, int ppi);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetSourceYResolution")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiGetSourceYResolution(IntPtr handle);

    #endregion

    #region BaseAPI Image Setting

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetImage(IntPtr handle, IntPtr imagedata, uint width, uint height,
        uint bytesPerPixel, uint bytesPerLine);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetImage2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetImage2(IntPtr handle, IntPtr pix);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetMinOrientationMargin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetMinOrientationMargin(IntPtr handle, double margin);

    #endregion

    #region BaseAPI Recognition

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIRecognize")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiRecognize(IntPtr handle, IntPtr monitor);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIProcessPages", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiProcessPages(IntPtr handle, string filename, string retryConfig,
        int timeoutMillis, IntPtr renderer);

    #endregion

    #region BaseAPI Text Output

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetUTF8Text")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseAPIGetUTF8Text(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetHOCRText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetHOCRText(IntPtr handle, int pageNumber);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetAltoText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetAltoText(IntPtr handle, int pageNumber);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetTsvText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetTsvText(IntPtr handle, int pageNumber);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetLSTMBoxText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetLSTMBoxText(IntPtr handle, int pageNumber);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetWordStrBoxText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetWordStrBoxText(IntPtr handle, int pageNumber);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetUNLVText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetUNLVText(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetOsdText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetOsdText(IntPtr handle, int pageNumber);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetBestLSTMBoxText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetBestLSTMBoxText(IntPtr handle, int pageNumber);

    #endregion

    #region BaseAPI Text Deletion

    [LibraryImport(DllName, EntryPoint = "TessDeleteText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteText(IntPtr text);

    [LibraryImport(DllName, EntryPoint = "TessDeleteTextArray")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteTextArray(IntPtr arr);

    [LibraryImport(DllName, EntryPoint = "TessDeleteIntArray")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteIntArray(IntPtr arr);

    [LibraryImport(DllName, EntryPoint = "TessDeleteBlockList")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteBlockList(IntPtr blockList);

    #endregion

    #region BaseAPI Confidence

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIMeanTextConf")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiMeanTextConf(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIAllWordConfidences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiAllWordConfidences(IntPtr handle);

    #endregion

    #region BaseAPI Analysis

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIAnalyseLayout")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiAnalyseLayout(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIDetectOrientationScript",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiDetectOrientationScript(IntPtr handle, out int orientDeg,
        out float orientConf, out string scriptName, out float scriptConf);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIDetectOS")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiDetectOs(IntPtr handle, out OrientationPage orientation,
        out WritingDirection writingDirection, out TextlineOrder textlineOrder, out float deskewAngle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetTextDirection")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetTextDirection(IntPtr handle, out int offset, out float slope);

    #endregion

    #region BaseAPI Image Retrieval

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetThresholdedImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetThresholdedImage(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetThresholdedImageScaleFactor")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiGetThresholdedImageScaleFactor(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetRegions")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetRegions(IntPtr handle, out IntPtr pixa);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetTextlines")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetTextlines(IntPtr handle, out IntPtr pixa, out IntPtr blockIds);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetStrips")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetStrips(IntPtr handle, out IntPtr pixa, out IntPtr blockIds);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetWords")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetWords(IntPtr handle, out IntPtr pixa);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetCharacters")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetCharacters(IntPtr handle, out IntPtr pixa);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetComponentImages")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetComponentImages(IntPtr handle, PageIteratorLevel level,
        [MarshalAs(UnmanagedType.Bool)] bool textOnly, out IntPtr pixa, out IntPtr blockIds);

    #endregion

    #region BaseAPI Dictionary

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIIsValidWord", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiIsValidWord(IntPtr handle, string word);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIAdaptToWordStr", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiAdaptToWordStr(IntPtr handle, PageSegmentMode mode, string wordStr);

    #endregion

    #region BaseAPI Clear

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIClear")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiClear(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIClearPersistentCache")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiClearPersistentCache(IntPtr handle);

    #endregion

    #region BaseAPI Languages

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetAvailableLanguagesAsVector")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetAvailableLanguagesAsVector(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetUnichar")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetUnichar(IntPtr handle, int unicharId);

    #endregion

    #region BaseAPI LSTM

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetLSTMChoice")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetLstmChoice(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetLSTMTimestep")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetLstmTimestep(IntPtr handle);

    #endregion

    #region BaseAPI Adaptive Classifier

    [LibraryImport(DllName, EntryPoint = "TessBaseAPISetAdaptiveClassifier")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetAdaptiveClassifier(IntPtr handle,
        [MarshalAs(UnmanagedType.Bool)] bool enable);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetAdaptiveClassifier")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiGetAdaptiveClassifier(IntPtr handle);

    #endregion

    #region BaseAPI Features (Training)

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetFeaturesForBlob")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetFeaturesForBlob(IntPtr handle, IntPtr blob, out int featureSize);

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIFreeFeatures")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiFreeFeatures(IntPtr handle, IntPtr features);

    #endregion

    #region PageIterator

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorBegin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorBegin(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorNext(IntPtr iterator, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorIsAtBeginningOf(IntPtr iterator, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorIsAtFinalElement")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessPageIteratorIsAtFinalElement(IntPtr iterator, PageIteratorLevel level,
        PageIteratorLevel element);

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

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorCopy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessPageIteratorCopy(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessPageIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorDelete(IntPtr iterator);

    #endregion

    #region ResultIterator

    [LibraryImport(DllName, EntryPoint = "TessBaseAPIGetIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBaseApiGetIterator(IntPtr handle);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorCopy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorCopy(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessResultIteratorDelete(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorNext(IntPtr iterator, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorIsAtFinalElement")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorIsAtFinalElement(IntPtr iterator, PageIteratorLevel level,
        PageIteratorLevel element);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetUTF8Text")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetUtf8Text(IntPtr iterator, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetConfidence")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float TessResultIteratorGetConfidence(IntPtr iterator, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorWordConfidences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorWordConfidences(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorWordRecognitionLanguage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorWordRecognitionLanguage(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorWordFontAttributes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorWordFontAttributes(
        IntPtr iterator,
        [MarshalAs(UnmanagedType.Bool)] out bool isBold,
        [MarshalAs(UnmanagedType.Bool)] out bool isItalic,
        [MarshalAs(UnmanagedType.Bool)] out bool isUnderlined,
        [MarshalAs(UnmanagedType.Bool)] out bool isMonospace,
        [MarshalAs(UnmanagedType.Bool)] out bool isSerif,
        [MarshalAs(UnmanagedType.Bool)] out bool isSmallCaps,
        out int pointSize,
        out int fontId);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorWordIsFromDictionary")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorWordIsFromDictionary(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorWordIsNumeric")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorWordIsNumeric(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorSymbolIsSuperscript")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorSymbolIsSuperscript(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorSymbolIsSubscript")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorSymbolIsSubscript(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorSymbolIsDropcap")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorSymbolIsDropcap(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetWordStrAttributes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetWordStrAttributes(IntPtr iterator,
        out int isBold, out int isItalic, out int isUnderlined, out int isMonospace, out int isSerif,
        out int isSmallCaps, out int pointSize, out int fontId);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetWordLSTMChoice")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetWordLstmChoice(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetWordTimestep")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetWordTimestep(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetPageIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetPageIterator(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetPageIteratorConst")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetPageIteratorConst(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetImage(IntPtr iterator, PageIteratorLevel level, int padding,
        IntPtr originalImage, out int left, out int top);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorBoundingBox")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorBoundingBox(IntPtr iterator, PageIteratorLevel level,
        out int left, out int top, out int right, out int bottom);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorBaseline")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorBaseline(IntPtr iterator, PageIteratorLevel level,
        out int x1, out int y1, out int x2, out int y2);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorBlockType")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial PolyBlockType TessResultIteratorBlockType(IntPtr iterator);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetBinaryImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetBinaryImage(IntPtr iterator, PageIteratorLevel level);

    [LibraryImport(DllName, EntryPoint = "TessResultIteratorGetChoiceIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultIteratorGetChoiceIterator(IntPtr iterator);

    #endregion

    #region ChoiceIterator

    [LibraryImport(DllName, EntryPoint = "TessChoiceIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessChoiceIteratorDelete(IntPtr choiceIterator);

    [LibraryImport(DllName, EntryPoint = "TessChoiceIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessChoiceIteratorNext(IntPtr choiceIterator);

    [LibraryImport(DllName, EntryPoint = "TessChoiceIteratorGetUTF8Text")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessChoiceIteratorGetUtf8Text(IntPtr choiceIterator);

    [LibraryImport(DllName, EntryPoint = "TessChoiceIteratorGetConfidence")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float TessChoiceIteratorGetConfidence(IntPtr choiceIterator);

    [LibraryImport(DllName, EntryPoint = "TessChoiceIteratorGetWordConfidences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessChoiceIteratorGetWordConfidences(IntPtr choiceIterator);

    #endregion

    #region Monitor

    [LibraryImport(DllName, EntryPoint = "TessMonitorCreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessMonitorCreate();

    [LibraryImport(DllName, EntryPoint = "TessMonitorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorDelete(IntPtr monitor);

    [LibraryImport(DllName, EntryPoint = "TessMonitorSetCancelFunc")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetCancelFunc(IntPtr monitor, IntPtr cancelFunc);

    [LibraryImport(DllName, EntryPoint = "TessMonitorGetCancelThis")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessMonitorGetCancelThis(IntPtr monitor);

    [LibraryImport(DllName, EntryPoint = "TessMonitorSetCancelThis")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetCancelThis(IntPtr monitor, IntPtr cancelThis);

    [LibraryImport(DllName, EntryPoint = "TessMonitorGetProgress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessMonitorGetProgress(IntPtr monitor);

    [LibraryImport(DllName, EntryPoint = "TessMonitorSetProgress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetProgress(IntPtr monitor, int progress);

    #endregion

    #region Renderers

    [LibraryImport(DllName, EntryPoint = "TessTextRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessTextRendererCreate(string outputBase);

    [LibraryImport(DllName, EntryPoint = "TessHOcrRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessHOcrRendererCreate(string outputBase);

    [LibraryImport(DllName, EntryPoint = "TessHOcrRendererCreate2", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessHOcrRendererCreate2(string outputBase,
        [MarshalAs(UnmanagedType.Bool)] bool fontInfo);

    [LibraryImport(DllName, EntryPoint = "TessAltoRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessAltoRendererCreate(string outputBase);

    [LibraryImport(DllName, EntryPoint = "TessTsvRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessTsvRendererCreate(string outputBase);

    [LibraryImport(DllName, EntryPoint = "TessPDFRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessPdfRendererCreate(string outputBase, string dataDir,
        [MarshalAs(UnmanagedType.Bool)] bool textOnly);

    [LibraryImport(DllName, EntryPoint = "TessUnlvRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessUnlvRendererCreate(string outputBase);

    [LibraryImport(DllName, EntryPoint = "TessBoxTextRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessBoxTextRendererCreate(string outputBase);

    [LibraryImport(DllName, EntryPoint = "TessLSTMBoxRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessLstmBoxRendererCreate(string outputBase);

    [LibraryImport(DllName, EntryPoint = "TessWordStrBoxRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessWordStrBoxRendererCreate(string outputBase);

    [LibraryImport(DllName, EntryPoint = "TessDeleteResultRenderer")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteResultRenderer(IntPtr renderer);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererInsert")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessResultRendererInsert(IntPtr renderer, IntPtr subRenderer);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultRendererNext(IntPtr renderer);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererBeginDocument", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultRendererBeginDocument(IntPtr renderer, string title);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererAddImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultRendererAddImage(IntPtr renderer, IntPtr api);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererEndDocument")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultRendererEndDocument(IntPtr renderer);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererExtention")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultRendererExtention(IntPtr renderer);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererTitle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultRendererTitle(IntPtr renderer);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererImageNum")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessResultRendererImageNum(IntPtr renderer);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererOutputType")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr TessResultRendererOutputType(IntPtr renderer);

    [LibraryImport(DllName, EntryPoint = "TessResultRendererSetPermissions")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessResultRendererSetPermissions(IntPtr renderer, int permissions);

    #endregion
}