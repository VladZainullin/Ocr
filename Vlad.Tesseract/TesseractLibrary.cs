using System.Runtime.InteropServices;
using System.Text;

namespace Vlad.Tesseract;

public unsafe class TesseractLibrary : IDisposable
{
    private readonly nint _libraryHandle;
    private bool _disposed;

    // Version
    private readonly delegate* unmanaged[Cdecl]<nint> _tessVersion;

    // BaseAPI Lifecycle
    private readonly delegate* unmanaged[Cdecl]<nint> _tessBaseApiCreate;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessBaseApiDelete;

    // BaseAPI Initialization
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, int> _tessBaseApiInit3;

    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, int, nint, nint, nint, byte, int>
        _tessBaseApiInit4;

    // BaseAPI Configuration
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte> _tessBaseApiSetVariable;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte> _tessBaseApiSetDebugVariable;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, int*, byte> _tessBaseApiGetIntVariable;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte> _tessBaseApiGetBoolVariable;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, double*, byte> _tessBaseApiGetDoubleVariable;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, nint> _tessBaseApiGetStringVariable;
    private readonly delegate* unmanaged[Cdecl]<nint, nint*, nint> _tessBaseApiGetOpenClDevice;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, void> _tessBaseApiReadConfigFile;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, void> _tessBaseApiReadDebugConfigFile;

    // BaseAPI Debug
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessBaseApiPrintVariables;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte> _tessBaseApiPrintVariablesToFile;

    // BaseAPI Page Segmentation
    private readonly delegate* unmanaged[Cdecl]<nint, PageSegmentMode, void> _tessBaseApiSetPageSegMode;
    private readonly delegate* unmanaged[Cdecl]<nint, PageSegmentMode> _tessBaseApiGetPageSegMode;

    // BaseAPI Input/Output Names
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, void> _tessBaseApiSetInputName;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetInputName;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, void> _tessBaseApiSetOutputName;

    // BaseAPI Source Resolution
    private readonly delegate* unmanaged[Cdecl]<nint, int, void> _tessBaseApiSetSourceResolution;
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessBaseApiGetSourceYResolution;

    // BaseAPI Image Setting
    private readonly delegate* unmanaged[Cdecl]<nint, nint, uint, uint, uint, uint, void> _tessBaseApiSetImage;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessBaseApiSetImage2;
    private readonly delegate* unmanaged[Cdecl]<nint, double, void> _tessBaseApiSetMinOrientationMargin;

    // BaseAPI Recognition
    private readonly delegate* unmanaged[Cdecl]<nint, nint, int> _tessBaseApiRecognize;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, byte> _tessBaseApiProcessPages;

    // BaseAPI Text Output
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetUtf8Text;
    private readonly delegate* unmanaged[Cdecl]<nint, int, nint> _tessBaseApiGetHocrText;
    private readonly delegate* unmanaged[Cdecl]<nint, int, nint> _tessBaseApiGetAltoText;
    private readonly delegate* unmanaged[Cdecl]<nint, int, nint> _tessBaseApiGetTsvText;
    private readonly delegate* unmanaged[Cdecl]<nint, int, nint> _tessBaseApiGetLstmBoxText;
    private readonly delegate* unmanaged[Cdecl]<nint, int, nint> _tessBaseApiGetWordStrBoxText;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetUnlvText;

    // BaseAPI Text Deletion
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessDeleteText;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessDeleteTextArray;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessDeleteIntArray;

    // BaseAPI Confidence
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessBaseApiMeanTextConf;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiAllWordConfidences;

    // BaseAPI Analysis
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessBaseApiAnalyseLayout;

    private readonly delegate* unmanaged[Cdecl]<nint, int*, float*, byte**, float*, byte>
        _tessBaseApiDetectOrientationScript;

    private readonly delegate* unmanaged[Cdecl]<nint, int*, float*, nint> _tessBaseApiGetTextDirection;

    // BaseAPI Image Retrieval
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetThresholdedImage;
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessBaseApiGetThresholdedImageScaleFactor;

    // BaseAPI Dictionary
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, int> _tessBaseApiIsValidWord;
    private readonly delegate* unmanaged[Cdecl]<nint, PageSegmentMode, byte*, byte> _tessBaseApiAdaptToWordStr;

    // BaseAPI Clear
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessBaseApiClear;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessBaseApiClearPersistentCache;

    // BaseAPI Languages
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetAvailableLanguagesAsVector;
    private readonly delegate* unmanaged[Cdecl]<nint, int, nint> _tessBaseApiGetUnichar;

    // PageIterator
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessPageIteratorBegin;
    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte> _tessPageIteratorNext;
    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte> _tessPageIteratorIsAtBeginningOf;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, PageIteratorLevel, int>
        _tessPageIteratorIsAtFinalElement;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>
        _tessPageIteratorBoundingBox;

    private readonly delegate* unmanaged[Cdecl]<nint, PolyBlockType> _tessPageIteratorBlockType;
    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint> _tessPageIteratorGetBinaryImage;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int, nint, int*, int*, nint>
        _tessPageIteratorGetImage;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>
        _tessPageIteratorBaseline;

    private readonly delegate* unmanaged[Cdecl]<nint, OrientationPage*, WritingDirection*, TextlineOrder*, float*, void>
        _tessPageIteratorOrientation;

    private readonly delegate* unmanaged[Cdecl]<nint, ParagraphJustification*, byte*, byte*, int*, void>
        _tessPageIteratorParagraphInfo;

    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessPageIteratorCopy;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessPageIteratorDelete;

    // ResultIterator
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetIterator;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorCopy;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessResultIteratorDelete;
    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte> _tessResultIteratorNext;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint> _tessResultIteratorGetUtf8Text;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorWordRecognitionLanguage;

    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte*, byte*, byte*, byte*, int*, int*, byte>
        _tessResultIteratorWordFontAttributes;

    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorWordIsFromDictionary;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorWordIsNumeric;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorSymbolIsSuperscript;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorSymbolIsSubscript;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorSymbolIsDropcap;

    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorGetPageIterator;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorGetPageIteratorConst;

    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorGetChoiceIterator;

    // ChoiceIterator
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessChoiceIteratorDelete;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessChoiceIteratorNext;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessChoiceIteratorGetUtf8Text;

    // Monitor
    private readonly delegate* unmanaged[Cdecl]<nint> _tessMonitorCreate;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessMonitorDelete;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessMonitorSetCancelFunc;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessMonitorGetCancelThis;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessMonitorSetCancelThis;
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessMonitorGetProgress;

    // Renderers
    private readonly delegate* unmanaged[Cdecl]<byte*, nint> _tessTextRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<byte*, nint> _tessHOcrRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<byte*, byte, nint> _tessHOcrRendererCreate2;
    private readonly delegate* unmanaged[Cdecl]<byte*, nint> _tessAltoRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<byte*, nint> _tessTsvRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<byte*, byte*, byte, nint> _tessPdfRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<byte*, nint> _tessUnlvRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<byte*, nint> _tessBoxTextRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<byte*, nint> _tessLstmBoxRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<byte*, nint> _tessWordStrBoxRendererCreate;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessDeleteResultRenderer;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessResultRendererInsert;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultRendererNext;
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte> _tessResultRendererBeginDocument;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, byte> _tessResultRendererAddImage;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultRendererEndDocument;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultRendererExtention;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultRendererTitle;
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessResultRendererImageNum;

    public TesseractLibrary(string dllPath)
    {
        if (!File.Exists(dllPath)) throw new FileNotFoundException($"Library not found: {dllPath}");

        _libraryHandle = NativeLibrary.Load(dllPath);

        // Version
        _tessVersion = (delegate* unmanaged[Cdecl]<nint>)NativeLibrary.GetExport(_libraryHandle, "TessVersion");

        // BaseAPI Lifecycle
        _tessBaseApiCreate =
            (delegate* unmanaged[Cdecl]<nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPICreate");

        _tessBaseApiDelete = (delegate* unmanaged[Cdecl]<nint, void>)
            NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIDelete");

        _tessBaseApiInit3 = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int>)
            NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIInit3");

        _tessBaseApiInit4 =
            (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, int, nint, nint, nint, byte, int>)
            NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIInit4");

        // BaseAPI Configuration
        _tessBaseApiSetVariable = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetVariable");

        _tessBaseApiSetDebugVariable = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetDebugVariable");

        _tessBaseApiGetIntVariable = (delegate* unmanaged[Cdecl]<nint, byte*, int*, byte>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetIntVariable");

        _tessBaseApiGetBoolVariable = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetBoolVariable");

        _tessBaseApiGetDoubleVariable = (delegate* unmanaged[Cdecl]<nint, byte*, double*, byte>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetDoubleVariable");

        _tessBaseApiGetStringVariable = (delegate* unmanaged[Cdecl]<nint, byte*, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetStringVariable");

        _tessBaseApiGetOpenClDevice = (delegate* unmanaged[Cdecl]<nint, nint*, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetOpenCLDevice");

        _tessBaseApiReadConfigFile = (delegate* unmanaged[Cdecl]<nint, byte*, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIReadConfigFile");

        _tessBaseApiReadDebugConfigFile = (delegate* unmanaged[Cdecl]<nint, byte*, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIReadDebugConfigFile");

        // BaseAPI Debug
        _tessBaseApiPrintVariables = (delegate* unmanaged[Cdecl]<nint, nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIPrintVariables");

        _tessBaseApiPrintVariablesToFile = (delegate* unmanaged[Cdecl]<nint, byte*, byte>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIPrintVariablesToFile");

        // BaseAPI Page Segmentation
        _tessBaseApiSetPageSegMode = (delegate* unmanaged[Cdecl]<nint, PageSegmentMode, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetPageSegMode");

        _tessBaseApiGetPageSegMode = (delegate* unmanaged[Cdecl]<nint, PageSegmentMode>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetPageSegMode");

        // BaseAPI Input/Output Names
        _tessBaseApiSetInputName = (delegate* unmanaged[Cdecl]<nint, byte*, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetInputName");

        _tessBaseApiGetInputName = (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetInputName");

        _tessBaseApiSetOutputName = (delegate* unmanaged[Cdecl]<nint, byte*, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetOutputName");

        // BaseAPI Source Resolution
        _tessBaseApiSetSourceResolution = (delegate* unmanaged[Cdecl]<nint, int, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetSourceResolution");

        _tessBaseApiGetSourceYResolution = (delegate* unmanaged[Cdecl]<nint, int>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetSourceYResolution");

        // BaseAPI Image Setting
        _tessBaseApiSetImage = (delegate* unmanaged[Cdecl]<nint, nint, uint, uint, uint, uint, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetImage");

        _tessBaseApiSetImage2 = (delegate* unmanaged[Cdecl]<nint, nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetImage2");

        _tessBaseApiSetMinOrientationMargin =
            (delegate* unmanaged[Cdecl]<nint, double, void>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetMinOrientationMargin");

        // BaseAPI Recognition
        _tessBaseApiRecognize = (delegate* unmanaged[Cdecl]<nint, nint, int>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIRecognize");

        _tessBaseApiProcessPages = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, byte>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIProcessPages");

        // BaseAPI Text Output
        _tessBaseApiGetUtf8Text = (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetUTF8Text");

        _tessBaseApiGetHocrText = (delegate* unmanaged[Cdecl]<nint, int, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetHOCRText");

        _tessBaseApiGetAltoText = (delegate* unmanaged[Cdecl]<nint, int, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetAltoText");

        _tessBaseApiGetTsvText = (delegate* unmanaged[Cdecl]<nint, int, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetTsvText");

        _tessBaseApiGetLstmBoxText = (delegate* unmanaged[Cdecl]<nint, int, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetLSTMBoxText");

        _tessBaseApiGetWordStrBoxText = (delegate* unmanaged[Cdecl]<nint, int, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetWordStrBoxText");

        _tessBaseApiGetUnlvText = (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetUNLVText");

        // BaseAPI Text Deletion
        _tessDeleteText = (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessDeleteText");

        _tessDeleteTextArray = (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessDeleteTextArray");

        _tessDeleteIntArray = (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessDeleteIntArray");

        // BaseAPI Confidence
        _tessBaseApiMeanTextConf = (delegate* unmanaged[Cdecl]<nint, int>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIMeanTextConf");

        _tessBaseApiAllWordConfidences = (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIAllWordConfidences");

        // BaseAPI Analysis
        _tessBaseApiAnalyseLayout =
            (delegate* unmanaged[Cdecl]<nint, int>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIAnalyseLayout");

        _tessBaseApiDetectOrientationScript =
            (delegate* unmanaged[Cdecl]<nint, int*, float*, byte**, float*, byte>)NativeLibrary.GetExport(
                _libraryHandle, "TessBaseAPIDetectOrientationScript");

        _tessBaseApiGetTextDirection =
            (delegate* unmanaged[Cdecl]<nint, int*, float*, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessBaseAPIGetTextDirection");

        // BaseAPI Image Retrieval
        _tessBaseApiGetThresholdedImage =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessBaseAPIGetThresholdedImage");

        _tessBaseApiGetThresholdedImageScaleFactor =
            (delegate* unmanaged[Cdecl]<nint, int>)NativeLibrary.GetExport(_libraryHandle,
                "TessBaseAPIGetThresholdedImageScaleFactor");

        // BaseAPI Dictionary
        _tessBaseApiIsValidWord =
            (delegate* unmanaged[Cdecl]<nint, byte*, int>)NativeLibrary.GetExport(_libraryHandle,
                "TessBaseAPIIsValidWord");

        _tessBaseApiAdaptToWordStr =
            (delegate* unmanaged[Cdecl]<nint, PageSegmentMode, byte*, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessBaseAPIAdaptToWordStr");

        // BaseAPI Clear
        var clearPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIClear");
        _tessBaseApiClear = (delegate* unmanaged[Cdecl]<nint, void>)clearPtr;

        var clearPersistentCachePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIClearPersistentCache");
        _tessBaseApiClearPersistentCache = (delegate* unmanaged[Cdecl]<nint, void>)clearPersistentCachePtr;

        // BaseAPI Languages
        _tessBaseApiGetAvailableLanguagesAsVector =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessBaseAPIGetAvailableLanguagesAsVector");

        _tessBaseApiGetUnichar =
            (delegate* unmanaged[Cdecl]<nint, int, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessBaseAPIGetUnichar");

        // PageIterator
        _tessPageIteratorBegin =
            (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorBegin");

        _tessPageIteratorNext =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessPageIteratorNext");

        _tessPageIteratorIsAtBeginningOf =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessPageIteratorIsAtBeginningOf");

        _tessPageIteratorIsAtFinalElement =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, PageIteratorLevel, int>)
            NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorIsAtFinalElement");

        _tessPageIteratorBoundingBox =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>)
            NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorBoundingBox");

        _tessPageIteratorBlockType =
            (delegate* unmanaged[Cdecl]<nint, PolyBlockType>)NativeLibrary.GetExport(_libraryHandle,
                "TessPageIteratorBlockType");

        _tessPageIteratorGetBinaryImage =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessPageIteratorGetBinaryImage");

        var pageIteratorGetImagePtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorGetImage");
        _tessPageIteratorGetImage =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int, nint, int*, int*, nint>)pageIteratorGetImagePtr;

        _tessPageIteratorBaseline =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>)NativeLibrary.GetExport(
                _libraryHandle, "TessPageIteratorBaseline");

        _tessPageIteratorOrientation =
            (delegate* unmanaged[Cdecl]<nint, OrientationPage*, WritingDirection*, TextlineOrder*, float*, void>)
            NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorOrientation");

        _tessPageIteratorParagraphInfo =
            (delegate* unmanaged[Cdecl]<nint, ParagraphJustification*, byte*, byte*, int*, void>)
            NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorParagraphInfo");

        _tessPageIteratorCopy =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorCopy");

        _tessPageIteratorDelete =
            (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorDelete");

        // ResultIterator
        _tessBaseApiGetIterator =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetIterator");

        _tessResultIteratorCopy =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorCopy");

        _tessResultIteratorDelete =
            (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorDelete");

        _tessResultIteratorNext =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorNext");

        _tessResultIteratorGetUtf8Text =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorGetUTF8Text");

        _tessResultIteratorWordRecognitionLanguage =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorWordRecognitionLanguage");

        _tessResultIteratorWordFontAttributes =
            (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte*, byte*, byte*, byte*, int*, int*, byte>)
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorWordFontAttributes");

        _tessResultIteratorWordIsFromDictionary =
            (delegate* unmanaged[Cdecl]<nint, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorWordIsFromDictionary");

        _tessResultIteratorWordIsNumeric =
            (delegate* unmanaged[Cdecl]<nint, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorWordIsNumeric");

        _tessResultIteratorSymbolIsSuperscript =
            (delegate* unmanaged[Cdecl]<nint, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorSymbolIsSuperscript");

        _tessResultIteratorSymbolIsSubscript =
            (delegate* unmanaged[Cdecl]<nint, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorSymbolIsSubscript");

        _tessResultIteratorSymbolIsDropcap =
            (delegate* unmanaged[Cdecl]<nint, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorSymbolIsDropcap");

        _tessResultIteratorGetPageIterator =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorGetPageIterator");

        _tessResultIteratorGetPageIteratorConst =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorGetPageIteratorConst");

        _tessResultIteratorGetChoiceIterator =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultIteratorGetChoiceIterator");

        // ChoiceIterator
        _tessChoiceIteratorDelete =
            (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessChoiceIteratorDelete");

        _tessChoiceIteratorNext =
            (delegate* unmanaged[Cdecl]<nint, byte>)NativeLibrary.GetExport(_libraryHandle, "TessChoiceIteratorNext");

        _tessChoiceIteratorGetUtf8Text =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessChoiceIteratorGetUTF8Text");

        // Monitor
        _tessMonitorCreate =
            (delegate* unmanaged[Cdecl]<nint>)NativeLibrary.GetExport(_libraryHandle, "TessMonitorCreate");

        _tessMonitorDelete =
            (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessMonitorDelete");

        _tessMonitorSetCancelFunc =
            (delegate* unmanaged[Cdecl]<nint, nint, void>)NativeLibrary.GetExport(_libraryHandle,
                "TessMonitorSetCancelFunc");

        _tessMonitorGetCancelThis =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessMonitorGetCancelThis");

        _tessMonitorSetCancelThis =
            (delegate* unmanaged[Cdecl]<nint, nint, void>)NativeLibrary.GetExport(_libraryHandle,
                "TessMonitorSetCancelThis");

        _tessMonitorGetProgress =
            (delegate* unmanaged[Cdecl]<nint, int>)NativeLibrary.GetExport(_libraryHandle, "TessMonitorGetProgress");

        // Renderers
        _tessTextRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, nint>)NativeLibrary.GetExport(_libraryHandle, "TessTextRendererCreate");

        _tessHOcrRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, nint>)NativeLibrary.GetExport(_libraryHandle, "TessHOcrRendererCreate");

        _tessHOcrRendererCreate2 =
            (delegate* unmanaged[Cdecl]<byte*, byte, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessHOcrRendererCreate2");

        _tessAltoRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, nint>)NativeLibrary.GetExport(_libraryHandle, "TessAltoRendererCreate");

        _tessTsvRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, nint>)NativeLibrary.GetExport(_libraryHandle, "TessTsvRendererCreate");

        _tessPdfRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, byte*, byte, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessPDFRendererCreate");

        _tessUnlvRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, nint>)NativeLibrary.GetExport(_libraryHandle, "TessUnlvRendererCreate");

        _tessBoxTextRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessBoxTextRendererCreate");

        _tessLstmBoxRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessLSTMBoxRendererCreate");

        _tessWordStrBoxRendererCreate =
            (delegate* unmanaged[Cdecl]<byte*, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessWordStrBoxRendererCreate");

        _tessDeleteResultRenderer =
            (delegate* unmanaged[Cdecl]<nint, void>)NativeLibrary.GetExport(_libraryHandle, "TessDeleteResultRenderer");

        _tessResultRendererInsert =
            (delegate* unmanaged[Cdecl]<nint, nint, void>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultRendererInsert");

        _tessResultRendererNext =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessResultRendererNext");

        _tessResultRendererBeginDocument =
            (delegate* unmanaged[Cdecl]<nint, byte*, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultRendererBeginDocument");

        _tessResultRendererAddImage =
            (delegate* unmanaged[Cdecl]<nint, nint, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultRendererAddImage");

        _tessResultRendererEndDocument =
            (delegate* unmanaged[Cdecl]<nint, byte>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultRendererEndDocument");

        _tessResultRendererExtention =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultRendererExtention");

        _tessResultRendererTitle =
            (delegate* unmanaged[Cdecl]<nint, nint>)NativeLibrary.GetExport(_libraryHandle, "TessResultRendererTitle");

        _tessResultRendererImageNum =
            (delegate* unmanaged[Cdecl]<nint, int>)NativeLibrary.GetExport(_libraryHandle,
                "TessResultRendererImageNum");
    }

    // Version
    public nint TessVersion() => _tessVersion();

// BaseAPI Lifecycle
    public nint TessBaseApiCreate() => _tessBaseApiCreate();
    public void TessBaseApiDelete(nint handle) => _tessBaseApiDelete(handle);

// BaseAPI Initialization
    public bool TessBaseApiInit3(nint handle, ReadOnlySpan<char> dataPath, ReadOnlySpan<char> language)
    {
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

            return _tessBaseApiInit3(handle, pDataPath, pLanguage) == 0;
        }
        finally
        {
            if (freeDataPath) Marshal.FreeHGlobal((IntPtr)pDataPath);
            if (freeLanguage) Marshal.FreeHGlobal((IntPtr)pLanguage);
        }
    }


    public int TessBaseApiInit4(nint handle, byte* dataPath, byte* language, int mode, nint configs, int configsSize,
        nint varsVec, nint varsValues, nint varsVecSize, byte setOnlyNonDebugParams) => _tessBaseApiInit4(handle,
        dataPath, language, mode, configs, configsSize, varsVec, varsValues, varsVecSize, setOnlyNonDebugParams);

// BaseAPI Configuration
    public byte TessBaseApiSetVariable(nint handle, byte* name, byte* value) =>
        _tessBaseApiSetVariable(handle, name, value);

    public byte TessBaseApiSetDebugVariable(nint handle, byte* name, byte* value) =>
        _tessBaseApiSetDebugVariable(handle, name, value);

    public byte TessBaseApiGetIntVariable(nint handle, byte* name, int* value) =>
        _tessBaseApiGetIntVariable(handle, name, value);

    public byte TessBaseApiGetBoolVariable(nint handle, byte* name, byte* value) =>
        _tessBaseApiGetBoolVariable(handle, name, value);

    public byte TessBaseApiGetDoubleVariable(nint handle, byte* name, double* value) =>
        _tessBaseApiGetDoubleVariable(handle, name, value);

    public nint TessBaseApiGetStringVariable(nint handle, byte* name) => _tessBaseApiGetStringVariable(handle, name);
    public nint TessBaseApiGetOpenClDevice(nint handle, nint* device) => _tessBaseApiGetOpenClDevice(handle, device);
    public void TessBaseApiReadConfigFile(nint handle, byte* filename) => _tessBaseApiReadConfigFile(handle, filename);

    public void TessBaseApiReadDebugConfigFile(nint handle, byte* filename) =>
        _tessBaseApiReadDebugConfigFile(handle, filename);

// BaseAPI Debug
    public void TessBaseApiPrintVariables(nint handle, nint fp) => _tessBaseApiPrintVariables(handle, fp);

    public byte TessBaseApiPrintVariablesToFile(nint handle, byte* filename) =>
        _tessBaseApiPrintVariablesToFile(handle, filename);

// BaseAPI Page Segmentation
    public void TessBaseApiSetPageSegMode(nint handle, PageSegmentMode mode) =>
        _tessBaseApiSetPageSegMode(handle, mode);

    public PageSegmentMode TessBaseApiGetPageSegMode(nint handle) => _tessBaseApiGetPageSegMode(handle);

// BaseAPI Input/Output Names
    public void TessBaseApiSetInputName(nint handle, byte* name) => _tessBaseApiSetInputName(handle, name);
    public nint TessBaseApiGetInputName(nint handle) => _tessBaseApiGetInputName(handle);
    public void TessBaseApiSetOutputName(nint handle, byte* name) => _tessBaseApiSetOutputName(handle, name);

// BaseAPI Source Resolution
    public void TessBaseApiSetSourceResolution(nint handle, int ppi) => _tessBaseApiSetSourceResolution(handle, ppi);
    public int TessBaseApiGetSourceYResolution(nint handle) => _tessBaseApiGetSourceYResolution(handle);

// BaseAPI Image Setting
    public void TessBaseApiSetImage(nint handle, nint imagedata, uint width, uint height, uint bytesPerPixel,
        uint bytesPerLine) => _tessBaseApiSetImage(handle, imagedata, width, height, bytesPerPixel, bytesPerLine);

    public void TessBaseApiSetImage2(nint handle, nint pix) => _tessBaseApiSetImage2(handle, pix);

    public void TessBaseApiSetMinOrientationMargin(nint handle, double margin) =>
        _tessBaseApiSetMinOrientationMargin(handle, margin);

// BaseAPI Recognition
    public int TessBaseApiRecognize(nint handle, nint monitor) => _tessBaseApiRecognize(handle, monitor);

    public byte TessBaseApiProcessPages(nint handle, byte* filename, byte* retryConfig, int timeoutMillis,
        nint renderer) => _tessBaseApiProcessPages(handle, filename, retryConfig, timeoutMillis, renderer);

// BaseAPI Text Output
    public nint TessBaseApiGetUtf8Text(nint handle) => _tessBaseApiGetUtf8Text(handle);
    public nint TessBaseApiGetHocrText(nint handle, int pageNumber) => _tessBaseApiGetHocrText(handle, pageNumber);
    public nint TessBaseApiGetAltoText(nint handle, int pageNumber) => _tessBaseApiGetAltoText(handle, pageNumber);
    public nint TessBaseApiGetTsvText(nint handle, int pageNumber) => _tessBaseApiGetTsvText(handle, pageNumber);

    public nint TessBaseApiGetLstmBoxText(nint handle, int pageNumber) =>
        _tessBaseApiGetLstmBoxText(handle, pageNumber);

    public nint TessBaseApiGetWordStrBoxText(nint handle, int pageNumber) =>
        _tessBaseApiGetWordStrBoxText(handle, pageNumber);

    public nint TessBaseApiGetUnlvText(nint handle) => _tessBaseApiGetUnlvText(handle);

// BaseAPI Text Deletion
    public void TessDeleteText(nint text) => _tessDeleteText(text);
    public void TessDeleteTextArray(nint arr) => _tessDeleteTextArray(arr);
    public void TessDeleteIntArray(nint arr) => _tessDeleteIntArray(arr);

// BaseAPI Confidence
    public int TessBaseApiMeanTextConf(nint handle) => _tessBaseApiMeanTextConf(handle);
    public nint TessBaseApiAllWordConfidences(nint handle) => _tessBaseApiAllWordConfidences(handle);

// BaseAPI Analysis
    public int TessBaseApiAnalyseLayout(nint handle) => _tessBaseApiAnalyseLayout(handle);

    public byte TessBaseApiDetectOrientationScript(nint handle, int* orientDeg, float* orientConf, byte** scriptName,
        float* scriptConf) =>
        _tessBaseApiDetectOrientationScript(handle, orientDeg, orientConf, scriptName, scriptConf);

    public nint TessBaseApiGetTextDirection(nint handle, int* offset, float* slope) =>
        _tessBaseApiGetTextDirection(handle, offset, slope);

// BaseAPI Image Retrieval
    public nint TessBaseApiGetThresholdedImage(nint handle) => _tessBaseApiGetThresholdedImage(handle);

    public int TessBaseApiGetThresholdedImageScaleFactor(nint handle) =>
        _tessBaseApiGetThresholdedImageScaleFactor(handle);

// BaseAPI Dictionary
    public int TessBaseApiIsValidWord(nint handle, byte* word) => _tessBaseApiIsValidWord(handle, word);

    public byte TessBaseApiAdaptToWordStr(nint handle, PageSegmentMode mode, byte* wordStr) =>
        _tessBaseApiAdaptToWordStr(handle, mode, wordStr);

// BaseAPI Clear
    public void TessBaseApiClear(nint handle) => _tessBaseApiClear(handle);
    public void TessBaseApiClearPersistentCache(nint handle) => _tessBaseApiClearPersistentCache(handle);

// BaseAPI Languages
    public nint TessBaseApiGetAvailableLanguagesAsVector(nint handle) =>
        _tessBaseApiGetAvailableLanguagesAsVector(handle);

    public nint TessBaseApiGetUnichar(nint handle, int unicharId) => _tessBaseApiGetUnichar(handle, unicharId);

// PageIterator
    public void TessPageIteratorBegin(nint iterator) => _tessPageIteratorBegin(iterator);
    public byte TessPageIteratorNext(nint iterator, PageIteratorLevel level) => _tessPageIteratorNext(iterator, level);

    public byte TessPageIteratorIsAtBeginningOf(nint iterator, PageIteratorLevel level) =>
        _tessPageIteratorIsAtBeginningOf(iterator, level);

    public int TessPageIteratorIsAtFinalElement(nint iterator, PageIteratorLevel level, PageIteratorLevel element) =>
        _tessPageIteratorIsAtFinalElement(iterator, level, element);

    public byte TessPageIteratorBoundingBox(nint iterator, PageIteratorLevel level, int* left, int* top, int* right,
        int* bottom) => _tessPageIteratorBoundingBox(iterator, level, left, top, right, bottom);

    public PolyBlockType TessPageIteratorBlockType(nint iterator) => _tessPageIteratorBlockType(iterator);

    public nint TessPageIteratorGetBinaryImage(nint iterator, PageIteratorLevel level) =>
        _tessPageIteratorGetBinaryImage(iterator, level);

    public nint TessPageIteratorGetImage(nint iterator, PageIteratorLevel level, int padding, nint originalImage,
        int* left, int* top) => _tessPageIteratorGetImage(iterator, level, padding, originalImage, left, top);

    public byte TessPageIteratorBaseline(nint iterator, PageIteratorLevel level, int* x1, int* y1, int* x2, int* y2) =>
        _tessPageIteratorBaseline(iterator, level, x1, y1, x2, y2);

    public void TessPageIteratorOrientation(nint iterator, OrientationPage* orientation,
        WritingDirection* writingDirection, TextlineOrder* textlineOrder, float* deskewAngle) =>
        _tessPageIteratorOrientation(iterator, orientation, writingDirection, textlineOrder, deskewAngle);

    public void TessPageIteratorParagraphInfo(nint iterator, ParagraphJustification* justification, byte* isListItem,
        byte* isCrown, int* firstLineIndent) =>
        _tessPageIteratorParagraphInfo(iterator, justification, isListItem, isCrown, firstLineIndent);

    public nint TessPageIteratorCopy(nint iterator) => _tessPageIteratorCopy(iterator);
    public void TessPageIteratorDelete(nint iterator) => _tessPageIteratorDelete(iterator);

// ResultIterator
    public nint TessBaseApiGetIterator(nint handle) => _tessBaseApiGetIterator(handle);
    public nint TessResultIteratorCopy(nint iterator) => _tessResultIteratorCopy(iterator);
    public void TessResultIteratorDelete(nint iterator) => _tessResultIteratorDelete(iterator);

    public byte TessResultIteratorNext(nint iterator, PageIteratorLevel level) =>
        _tessResultIteratorNext(iterator, level);

    public nint TessResultIteratorGetUtf8Text(nint iterator, PageIteratorLevel level) =>
        _tessResultIteratorGetUtf8Text(iterator, level);

    public nint TessResultIteratorWordRecognitionLanguage(nint iterator) =>
        _tessResultIteratorWordRecognitionLanguage(iterator);

    public byte TessResultIteratorWordFontAttributes(nint iterator, byte* isBold, byte* isItalic, byte* isUnderlined,
        byte* isMonospace, byte* isSerif, byte* isSmallCaps, int* pointSize, int* fontId) =>
        _tessResultIteratorWordFontAttributes(iterator, isBold, isItalic, isUnderlined, isMonospace, isSerif,
            isSmallCaps, pointSize, fontId);

    public byte TessResultIteratorWordIsFromDictionary(nint iterator) =>
        _tessResultIteratorWordIsFromDictionary(iterator);

    public byte TessResultIteratorWordIsNumeric(nint iterator) => _tessResultIteratorWordIsNumeric(iterator);

    public byte TessResultIteratorSymbolIsSuperscript(nint iterator) =>
        _tessResultIteratorSymbolIsSuperscript(iterator);

    public byte TessResultIteratorSymbolIsSubscript(nint iterator) => _tessResultIteratorSymbolIsSubscript(iterator);
    public byte TessResultIteratorSymbolIsDropcap(nint iterator) => _tessResultIteratorSymbolIsDropcap(iterator);

    public nint TessResultIteratorGetPageIterator(nint iterator) => _tessResultIteratorGetPageIterator(iterator);

    public nint TessResultIteratorGetPageIteratorConst(nint iterator) =>
        _tessResultIteratorGetPageIteratorConst(iterator);

    public nint TessResultIteratorGetChoiceIterator(nint iterator) => _tessResultIteratorGetChoiceIterator(iterator);

// ChoiceIterator
    public void TessChoiceIteratorDelete(nint choiceIterator) => _tessChoiceIteratorDelete(choiceIterator);
    public byte TessChoiceIteratorNext(nint choiceIterator) => _tessChoiceIteratorNext(choiceIterator);
    public nint TessChoiceIteratorGetUtf8Text(nint choiceIterator) => _tessChoiceIteratorGetUtf8Text(choiceIterator);

// Monitor
    public nint TessMonitorCreate() => _tessMonitorCreate();
    public void TessMonitorDelete(nint monitor) => _tessMonitorDelete(monitor);

    public void TessMonitorSetCancelFunc(nint monitor, nint cancelFunc) =>
        _tessMonitorSetCancelFunc(monitor, cancelFunc);

    public nint TessMonitorGetCancelThis(nint monitor) => _tessMonitorGetCancelThis(monitor);

    public void TessMonitorSetCancelThis(nint monitor, nint cancelThis) =>
        _tessMonitorSetCancelThis(monitor, cancelThis);

    public int TessMonitorGetProgress(nint monitor) => _tessMonitorGetProgress(monitor);

// Renderers
    public nint TessTextRendererCreate(byte* outputBase) => _tessTextRendererCreate(outputBase);
    public nint TessHOcrRendererCreate(byte* outputBase) => _tessHOcrRendererCreate(outputBase);

    public nint TessHOcrRendererCreate2(byte* outputBase, byte fontInfo) =>
        _tessHOcrRendererCreate2(outputBase, fontInfo);

    public nint TessAltoRendererCreate(byte* outputBase) => _tessAltoRendererCreate(outputBase);
    public nint TessTsvRendererCreate(byte* outputBase) => _tessTsvRendererCreate(outputBase);

    public nint TessPdfRendererCreate(byte* outputBase, byte* dataDir, byte textOnly) =>
        _tessPdfRendererCreate(outputBase, dataDir, textOnly);

    public nint TessUnlvRendererCreate(byte* outputBase) => _tessUnlvRendererCreate(outputBase);
    public nint TessBoxTextRendererCreate(byte* outputBase) => _tessBoxTextRendererCreate(outputBase);
    public nint TessLstmBoxRendererCreate(byte* outputBase) => _tessLstmBoxRendererCreate(outputBase);
    public nint TessWordStrBoxRendererCreate(byte* outputBase) => _tessWordStrBoxRendererCreate(outputBase);
    public void TessDeleteResultRenderer(nint renderer) => _tessDeleteResultRenderer(renderer);

    public void TessResultRendererInsert(nint renderer, nint subRenderer) =>
        _tessResultRendererInsert(renderer, subRenderer);

    public nint TessResultRendererNext(nint renderer) => _tessResultRendererNext(renderer);

    public byte TessResultRendererBeginDocument(nint renderer, byte* title) =>
        _tessResultRendererBeginDocument(renderer, title);

    public byte TessResultRendererAddImage(nint renderer, nint api) => _tessResultRendererAddImage(renderer, api);
    public byte TessResultRendererEndDocument(nint renderer) => _tessResultRendererEndDocument(renderer);
    public nint TessResultRendererExtension(nint renderer) => _tessResultRendererExtention(renderer);
    public nint TessResultRendererTitle(nint renderer) => _tessResultRendererTitle(renderer);
    public int TessResultRendererImageNum(nint renderer) => _tessResultRendererImageNum(renderer);

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