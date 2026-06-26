using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

public unsafe class TesseractLibrary : IDisposable
{
    private readonly nint _libraryHandle;
    private bool _disposed;

    // Version
    private readonly delegate* unmanaged[Cdecl]<nint> _tessVersion;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiVersion;

    // BaseAPI Lifecycle
    private readonly delegate* unmanaged[Cdecl]<nint> _tessBaseApiCreate;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessBaseApiDelete;

    // BaseAPI Initialization
    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, int> _tessBaseApiInit3;

    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, int, nint, nint, nint, byte, int>
        _tessBaseApiInit4;

    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, int, nint, nint, nint, byte, int>
        _tessBaseApiInit5;

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
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessBaseApiSetWarningHandler;

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
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetOutputName;

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
    private readonly delegate* unmanaged[Cdecl]<nint, int, nint> _tessBaseApiGetOsdText;
    private readonly delegate* unmanaged[Cdecl]<nint, int, nint> _tessBaseApiGetBestLstmBoxText;

    // BaseAPI Text Deletion
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessDeleteText;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessDeleteTextArray;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessDeleteIntArray;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessDeleteBlockList;

    // BaseAPI Confidence
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessBaseApiMeanTextConf;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiAllWordConfidences;

    // BaseAPI Analysis
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessBaseApiAnalyseLayout;

    private readonly delegate* unmanaged[Cdecl]<nint, int*, float*, byte**, float*, byte>
        _tessBaseApiDetectOrientationScript;

    private readonly delegate* unmanaged[Cdecl]<nint, OrientationPage*, WritingDirection*, TextlineOrder*, float*, nint>
        _tessBaseApiDetectOs;

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

    // BaseAPI LSTM
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetLstmChoice;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetLstmTimestep;

    // BaseAPI Adaptive Classifier
    private readonly delegate* unmanaged[Cdecl]<nint, byte, void> _tessBaseApiSetAdaptiveClassifier;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessBaseApiGetAdaptiveClassifier;

    // BaseAPI Features (Training)
    private readonly delegate* unmanaged[Cdecl]<nint, nint, int*, nint> _tessBaseApiGetFeaturesForBlob;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessBaseApiFreeFeatures;

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

    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte*, byte*, byte*, byte*, int*, int*, byte>
        _tessPageIteratorGetWordFontAttributes;

    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessPageIteratorCopy;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessPageIteratorDelete;

    // ResultIterator
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessBaseApiGetIterator;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorCopy;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessResultIteratorDelete;
    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte> _tessResultIteratorNext;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, PageIteratorLevel, byte>
        _tessResultIteratorIsAtFinalElement;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint> _tessResultIteratorGetUtf8Text;
    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, float> _tessResultIteratorGetConfidence;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorWordConfidences;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorWordRecognitionLanguage;

    private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte*, byte*, byte*, byte*, int*, int*, byte>
        _tessResultIteratorWordFontAttributes;

    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorWordIsFromDictionary;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorWordIsNumeric;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorSymbolIsSuperscript;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorSymbolIsSubscript;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessResultIteratorSymbolIsDropcap;

    private readonly delegate* unmanaged[Cdecl]<nint, int*, int*, int*, int*, int*, int*, int*, int*, nint>
        _tessResultIteratorGetWordStrAttributes;

    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorGetWordLstmChoice;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorGetWordTimestep;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorGetPageIterator;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorGetPageIteratorConst;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int, nint, int*, int*, nint>
        _tessResultIteratorGetImage;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>
        _tessResultIteratorBoundingBox;

    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>
        _tessResultIteratorBaseline;

    private readonly delegate* unmanaged[Cdecl]<nint, PolyBlockType> _tessResultIteratorBlockType;
    private readonly delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint> _tessResultIteratorGetBinaryImage;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultIteratorGetChoiceIterator;

    // ChoiceIterator
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessChoiceIteratorDelete;
    private readonly delegate* unmanaged[Cdecl]<nint, byte> _tessChoiceIteratorNext;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessChoiceIteratorGetUtf8Text;
    private readonly delegate* unmanaged[Cdecl]<nint, float> _tessChoiceIteratorGetConfidence;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessChoiceIteratorGetWordConfidences;

    // Monitor
    private readonly delegate* unmanaged[Cdecl]<nint> _tessMonitorCreate;
    private readonly delegate* unmanaged[Cdecl]<nint, void> _tessMonitorDelete;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessMonitorSetCancelFunc;
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessMonitorGetCancelThis;
    private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _tessMonitorSetCancelThis;
    private readonly delegate* unmanaged[Cdecl]<nint, int> _tessMonitorGetProgress;
    private readonly delegate* unmanaged[Cdecl]<nint, int, void> _tessMonitorSetProgress;

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
    private readonly delegate* unmanaged[Cdecl]<nint, nint> _tessResultRendererOutputType;
    private readonly delegate* unmanaged[Cdecl]<nint, int, void> _tessResultRendererSetPermissions;

    public TesseractLibrary(string dllPath)
    {
        if (!File.Exists(dllPath)) throw new FileNotFoundException($"Library not found: {dllPath}");

        _libraryHandle = NativeLibrary.Load(dllPath);
        if (_libraryHandle == nint.Zero) throw new DllNotFoundException($"Failed to load library: {dllPath}");

        // Version
        var versionPtr = NativeLibrary.GetExport(_libraryHandle, "TessVersion");
        if (versionPtr == nint.Zero) throw new EntryPointNotFoundException("TessVersion not found in the library");
        _tessVersion = (delegate* unmanaged[Cdecl]<nint>)versionPtr;

        var baseApiVersionPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIVersion");
        if (baseApiVersionPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIVersion not found in the library");
        _tessBaseApiVersion = (delegate* unmanaged[Cdecl]<nint, nint>)baseApiVersionPtr;

        // BaseAPI Lifecycle
        var baseApiCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPICreate");
        if (baseApiCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPICreate not found in the library");
        _tessBaseApiCreate = (delegate* unmanaged[Cdecl]<nint>)baseApiCreatePtr;

        var baseApiDeletePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIDelete");
        if (baseApiDeletePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIDelete not found in the library");
        _tessBaseApiDelete = (delegate* unmanaged[Cdecl]<nint, void>)baseApiDeletePtr;

        // BaseAPI Initialization
        var baseApiInit3Ptr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIInit3");
        if (baseApiInit3Ptr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIInit3 not found in the library");
        _tessBaseApiInit3 = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int>)baseApiInit3Ptr;

        var baseApiInit4Ptr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIInit4");
        if (baseApiInit4Ptr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIInit4 not found in the library");
        _tessBaseApiInit4 =
            (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, int, nint, nint, nint, byte, int>)
            baseApiInit4Ptr;

        var baseApiInit5Ptr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIInit5");
        if (baseApiInit5Ptr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIInit5 not found in the library");
        _tessBaseApiInit5 =
            (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, int, nint, nint, nint, byte, int>)
            baseApiInit5Ptr;

        // BaseAPI Configuration
        var setVariablePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetVariable");
        if (setVariablePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetVariable not found in the library");
        _tessBaseApiSetVariable = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte>)setVariablePtr;

        var setDebugVariablePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetDebugVariable");
        if (setDebugVariablePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetDebugVariable not found in the library");
        _tessBaseApiSetDebugVariable = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte>)setDebugVariablePtr;

        var getIntVariablePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetIntVariable");
        if (getIntVariablePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetIntVariable not found in the library");
        _tessBaseApiGetIntVariable = (delegate* unmanaged[Cdecl]<nint, byte*, int*, byte>)getIntVariablePtr;

        var getBoolVariablePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetBoolVariable");
        if (getBoolVariablePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetBoolVariable not found in the library");
        _tessBaseApiGetBoolVariable = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte>)getBoolVariablePtr;

        var getDoubleVariablePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetDoubleVariable");
        if (getDoubleVariablePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetDoubleVariable not found in the library");
        _tessBaseApiGetDoubleVariable = (delegate* unmanaged[Cdecl]<nint, byte*, double*, byte>)getDoubleVariablePtr;

        var getStringVariablePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetStringVariable");
        if (getStringVariablePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetStringVariable not found in the library");
        _tessBaseApiGetStringVariable = (delegate* unmanaged[Cdecl]<nint, byte*, nint>)getStringVariablePtr;

        var getOpenClDevicePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetOpenCLDevice");
        if (getOpenClDevicePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetOpenCLDevice not found in the library");
        _tessBaseApiGetOpenClDevice = (delegate* unmanaged[Cdecl]<nint, nint*, nint>)getOpenClDevicePtr;

        var readConfigFilePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIReadConfigFile");
        if (readConfigFilePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIReadConfigFile not found in the library");
        _tessBaseApiReadConfigFile = (delegate* unmanaged[Cdecl]<nint, byte*, void>)readConfigFilePtr;

        var readDebugConfigFilePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIReadDebugConfigFile");
        if (readDebugConfigFilePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIReadDebugConfigFile not found in the library");
        _tessBaseApiReadDebugConfigFile = (delegate* unmanaged[Cdecl]<nint, byte*, void>)readDebugConfigFilePtr;

        var setWarningHandlerPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetWarningHandler");
        if (setWarningHandlerPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetWarningHandler not found in the library");
        _tessBaseApiSetWarningHandler = (delegate* unmanaged[Cdecl]<nint, nint, void>)setWarningHandlerPtr;

        // BaseAPI Debug
        var printVariablesPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIPrintVariables");
        if (printVariablesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIPrintVariables not found in the library");
        _tessBaseApiPrintVariables = (delegate* unmanaged[Cdecl]<nint, nint, void>)printVariablesPtr;

        var printVariablesToFilePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIPrintVariablesToFile");
        if (printVariablesToFilePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIPrintVariablesToFile not found in the library");
        _tessBaseApiPrintVariablesToFile = (delegate* unmanaged[Cdecl]<nint, byte*, byte>)printVariablesToFilePtr;

        // BaseAPI Page Segmentation
        var setPageSegModePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetPageSegMode");
        if (setPageSegModePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetPageSegMode not found in the library");
        _tessBaseApiSetPageSegMode = (delegate* unmanaged[Cdecl]<nint, PageSegmentMode, void>)setPageSegModePtr;

        var getPageSegModePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetPageSegMode");
        if (getPageSegModePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetPageSegMode not found in the library");
        _tessBaseApiGetPageSegMode = (delegate* unmanaged[Cdecl]<nint, PageSegmentMode>)getPageSegModePtr;

        // BaseAPI Input/Output Names
        var setInputNamePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetInputName");
        if (setInputNamePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetInputName not found in the library");
        _tessBaseApiSetInputName = (delegate* unmanaged[Cdecl]<nint, byte*, void>)setInputNamePtr;

        var getInputNamePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetInputName");
        if (getInputNamePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetInputName not found in the library");
        _tessBaseApiGetInputName = (delegate* unmanaged[Cdecl]<nint, nint>)getInputNamePtr;

        var setOutputNamePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetOutputName");
        if (setOutputNamePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetOutputName not found in the library");
        _tessBaseApiSetOutputName = (delegate* unmanaged[Cdecl]<nint, byte*, void>)setOutputNamePtr;

        var getOutputNamePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetOutputName");
        if (getOutputNamePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetOutputName not found in the library");
        _tessBaseApiGetOutputName = (delegate* unmanaged[Cdecl]<nint, nint>)getOutputNamePtr;

        // BaseAPI Source Resolution
        var setSourceResolutionPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetSourceResolution");
        if (setSourceResolutionPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetSourceResolution not found in the library");
        _tessBaseApiSetSourceResolution = (delegate* unmanaged[Cdecl]<nint, int, void>)setSourceResolutionPtr;

        var getSourceYResolutionPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetSourceYResolution");
        if (getSourceYResolutionPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetSourceYResolution not found in the library");
        _tessBaseApiGetSourceYResolution = (delegate* unmanaged[Cdecl]<nint, int>)getSourceYResolutionPtr;

        // BaseAPI Image Setting
        var setImagePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetImage");
        if (setImagePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetImage not found in the library");
        _tessBaseApiSetImage = (delegate* unmanaged[Cdecl]<nint, nint, uint, uint, uint, uint, void>)setImagePtr;

        var setImage2Ptr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetImage2");
        if (setImage2Ptr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetImage2 not found in the library");
        _tessBaseApiSetImage2 = (delegate* unmanaged[Cdecl]<nint, nint, void>)setImage2Ptr;

        var setMinOrientationMarginPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetMinOrientationMargin");
        if (setMinOrientationMarginPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetMinOrientationMargin not found in the library");
        _tessBaseApiSetMinOrientationMargin =
            (delegate* unmanaged[Cdecl]<nint, double, void>)setMinOrientationMarginPtr;

        // BaseAPI Recognition
        var recognizePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIRecognize");
        if (recognizePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIRecognize not found in the library");
        _tessBaseApiRecognize = (delegate* unmanaged[Cdecl]<nint, nint, int>)recognizePtr;

        var processPagesPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIProcessPages");
        if (processPagesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIProcessPages not found in the library");
        _tessBaseApiProcessPages = (delegate* unmanaged[Cdecl]<nint, byte*, byte*, int, nint, byte>)processPagesPtr;

        // BaseAPI Text Output
        var getUtf8TextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetUTF8Text");
        if (getUtf8TextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetUTF8Text not found in the library");
        _tessBaseApiGetUtf8Text = (delegate* unmanaged[Cdecl]<nint, nint>)getUtf8TextPtr;

        var getHocrTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetHOCRText");
        if (getHocrTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetHOCRText not found in the library");
        _tessBaseApiGetHocrText = (delegate* unmanaged[Cdecl]<nint, int, nint>)getHocrTextPtr;

        var getAltoTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetAltoText");
        if (getAltoTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetAltoText not found in the library");
        _tessBaseApiGetAltoText = (delegate* unmanaged[Cdecl]<nint, int, nint>)getAltoTextPtr;

        var getTsvTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetTsvText");
        if (getTsvTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetTsvText not found in the library");
        _tessBaseApiGetTsvText = (delegate* unmanaged[Cdecl]<nint, int, nint>)getTsvTextPtr;

        var getLstmBoxTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetLSTMBoxText");
        if (getLstmBoxTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetLSTMBoxText not found in the library");
        _tessBaseApiGetLstmBoxText = (delegate* unmanaged[Cdecl]<nint, int, nint>)getLstmBoxTextPtr;

        var getWordStrBoxTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetWordStrBoxText");
        if (getWordStrBoxTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetWordStrBoxText not found in the library");
        _tessBaseApiGetWordStrBoxText = (delegate* unmanaged[Cdecl]<nint, int, nint>)getWordStrBoxTextPtr;

        var getUnlvTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetUNLVText");
        if (getUnlvTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetUNLVText not found in the library");
        _tessBaseApiGetUnlvText = (delegate* unmanaged[Cdecl]<nint, nint>)getUnlvTextPtr;

        var getOsdTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetOsdText");
        if (getOsdTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetOsdText not found in the library");
        _tessBaseApiGetOsdText = (delegate* unmanaged[Cdecl]<nint, int, nint>)getOsdTextPtr;

        var getBestLstmBoxTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetBestLSTMBoxText");
        if (getBestLstmBoxTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetBestLSTMBoxText not found in the library");
        _tessBaseApiGetBestLstmBoxText = (delegate* unmanaged[Cdecl]<nint, int, nint>)getBestLstmBoxTextPtr;

        // BaseAPI Text Deletion
        var deleteTextPtr = NativeLibrary.GetExport(_libraryHandle, "TessDeleteText");
        if (deleteTextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessDeleteText not found in the library");
        _tessDeleteText = (delegate* unmanaged[Cdecl]<nint, void>)deleteTextPtr;

        var deleteTextArrayPtr = NativeLibrary.GetExport(_libraryHandle, "TessDeleteTextArray");
        if (deleteTextArrayPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessDeleteTextArray not found in the library");
        _tessDeleteTextArray = (delegate* unmanaged[Cdecl]<nint, void>)deleteTextArrayPtr;

        var deleteIntArrayPtr = NativeLibrary.GetExport(_libraryHandle, "TessDeleteIntArray");
        if (deleteIntArrayPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessDeleteIntArray not found in the library");
        _tessDeleteIntArray = (delegate* unmanaged[Cdecl]<nint, void>)deleteIntArrayPtr;

        var deleteBlockListPtr = NativeLibrary.GetExport(_libraryHandle, "TessDeleteBlockList");
        if (deleteBlockListPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessDeleteBlockList not found in the library");
        _tessDeleteBlockList = (delegate* unmanaged[Cdecl]<nint, void>)deleteBlockListPtr;

        // BaseAPI Confidence
        var meanTextConfPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIMeanTextConf");
        if (meanTextConfPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIMeanTextConf not found in the library");
        _tessBaseApiMeanTextConf = (delegate* unmanaged[Cdecl]<nint, int>)meanTextConfPtr;

        var allWordConfidencesPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIAllWordConfidences");
        if (allWordConfidencesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIAllWordConfidences not found in the library");
        _tessBaseApiAllWordConfidences = (delegate* unmanaged[Cdecl]<nint, nint>)allWordConfidencesPtr;

        // BaseAPI Analysis
        var analyseLayoutPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIAnalyseLayout");
        if (analyseLayoutPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIAnalyseLayout not found in the library");
        _tessBaseApiAnalyseLayout = (delegate* unmanaged[Cdecl]<nint, int>)analyseLayoutPtr;

        var detectOrientationScriptPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIDetectOrientationScript");
        if (detectOrientationScriptPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIDetectOrientationScript not found in the library");
        _tessBaseApiDetectOrientationScript =
            (delegate* unmanaged[Cdecl]<nint, int*, float*, byte**, float*, byte>)detectOrientationScriptPtr;

        var detectOsPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIDetectOS");
        if (detectOsPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIDetectOS not found in the library");
        _tessBaseApiDetectOs =
            (delegate* unmanaged[Cdecl]<nint, OrientationPage*, WritingDirection*, TextlineOrder*, float*, nint>)
            detectOsPtr;

        var getTextDirectionPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetTextDirection");
        if (getTextDirectionPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetTextDirection not found in the library");
        _tessBaseApiGetTextDirection = (delegate* unmanaged[Cdecl]<nint, int*, float*, nint>)getTextDirectionPtr;

        // BaseAPI Image Retrieval
        var getThresholdedImagePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetThresholdedImage");
        if (getThresholdedImagePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetThresholdedImage not found in the library");
        _tessBaseApiGetThresholdedImage = (delegate* unmanaged[Cdecl]<nint, nint>)getThresholdedImagePtr;

        var getThresholdedImageScaleFactorPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetThresholdedImageScaleFactor");
        if (getThresholdedImageScaleFactorPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetThresholdedImageScaleFactor not found in the library");
        _tessBaseApiGetThresholdedImageScaleFactor =
            (delegate* unmanaged[Cdecl]<nint, int>)getThresholdedImageScaleFactorPtr;

        // BaseAPI Dictionary
        var isValidWordPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIIsValidWord");
        if (isValidWordPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIIsValidWord not found in the library");
        _tessBaseApiIsValidWord = (delegate* unmanaged[Cdecl]<nint, byte*, int>)isValidWordPtr;

        var adaptToWordStrPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIAdaptToWordStr");
        if (adaptToWordStrPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIAdaptToWordStr not found in the library");
        _tessBaseApiAdaptToWordStr = (delegate* unmanaged[Cdecl]<nint, PageSegmentMode, byte*, byte>)adaptToWordStrPtr;

        // BaseAPI Clear
        var clearPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIClear");
        if (clearPtr == nint.Zero) throw new EntryPointNotFoundException("TessBaseAPIClear not found in the library");
        _tessBaseApiClear = (delegate* unmanaged[Cdecl]<nint, void>)clearPtr;

        var clearPersistentCachePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIClearPersistentCache");
        if (clearPersistentCachePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIClearPersistentCache not found in the library");
        _tessBaseApiClearPersistentCache = (delegate* unmanaged[Cdecl]<nint, void>)clearPersistentCachePtr;

        // BaseAPI Languages
        var getAvailableLanguagesAsVectorPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetAvailableLanguagesAsVector");
        if (getAvailableLanguagesAsVectorPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetAvailableLanguagesAsVector not found in the library");
        _tessBaseApiGetAvailableLanguagesAsVector =
            (delegate* unmanaged[Cdecl]<nint, nint>)getAvailableLanguagesAsVectorPtr;

        var getUnicharPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetUnichar");
        if (getUnicharPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetUnichar not found in the library");
        _tessBaseApiGetUnichar = (delegate* unmanaged[Cdecl]<nint, int, nint>)getUnicharPtr;

        // BaseAPI LSTM
        var getLstmChoicePtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetLSTMChoice");
        if (getLstmChoicePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetLSTMChoice not found in the library");
        _tessBaseApiGetLstmChoice = (delegate* unmanaged[Cdecl]<nint, nint>)getLstmChoicePtr;

        var getLstmTimestepPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetLSTMTimestep");
        if (getLstmTimestepPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetLSTMTimestep not found in the library");
        _tessBaseApiGetLstmTimestep = (delegate* unmanaged[Cdecl]<nint, nint>)getLstmTimestepPtr;

        // BaseAPI Adaptive Classifier
        var setAdaptiveClassifierPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPISetAdaptiveClassifier");
        if (setAdaptiveClassifierPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPISetAdaptiveClassifier not found in the library");
        _tessBaseApiSetAdaptiveClassifier = (delegate* unmanaged[Cdecl]<nint, byte, void>)setAdaptiveClassifierPtr;

        var getAdaptiveClassifierPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetAdaptiveClassifier");
        if (getAdaptiveClassifierPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetAdaptiveClassifier not found in the library");
        _tessBaseApiGetAdaptiveClassifier = (delegate* unmanaged[Cdecl]<nint, byte>)getAdaptiveClassifierPtr;

        // BaseAPI Features (Training)
        var getFeaturesForBlobPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetFeaturesForBlob");
        if (getFeaturesForBlobPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetFeaturesForBlob not found in the library");
        _tessBaseApiGetFeaturesForBlob = (delegate* unmanaged[Cdecl]<nint, nint, int*, nint>)getFeaturesForBlobPtr;

        var freeFeaturesPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIFreeFeatures");
        if (freeFeaturesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIFreeFeatures not found in the library");
        _tessBaseApiFreeFeatures = (delegate* unmanaged[Cdecl]<nint, nint, void>)freeFeaturesPtr;

        // PageIterator
        var pageIteratorBeginPtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorBegin");
        if (pageIteratorBeginPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorBegin not found in the library");
        _tessPageIteratorBegin = (delegate* unmanaged[Cdecl]<nint, void>)pageIteratorBeginPtr;

        var pageIteratorNextPtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorNext");
        if (pageIteratorNextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorNext not found in the library");
        _tessPageIteratorNext = (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte>)pageIteratorNextPtr;

        var pageIteratorIsAtBeginningOfPtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorIsAtBeginningOf");
        if (pageIteratorIsAtBeginningOfPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorIsAtBeginningOf not found in the library");
        _tessPageIteratorIsAtBeginningOf =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte>)pageIteratorIsAtBeginningOfPtr;

        var pageIteratorIsAtFinalElementPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorIsAtFinalElement");
        if (pageIteratorIsAtFinalElementPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorIsAtFinalElement not found in the library");
        _tessPageIteratorIsAtFinalElement =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, PageIteratorLevel, int>)
            pageIteratorIsAtFinalElementPtr;

        var pageIteratorBoundingBoxPtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorBoundingBox");
        if (pageIteratorBoundingBoxPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorBoundingBox not found in the library");
        _tessPageIteratorBoundingBox =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>)
            pageIteratorBoundingBoxPtr;

        var pageIteratorBlockTypePtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorBlockType");
        if (pageIteratorBlockTypePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorBlockType not found in the library");
        _tessPageIteratorBlockType = (delegate* unmanaged[Cdecl]<nint, PolyBlockType>)pageIteratorBlockTypePtr;

        var pageIteratorGetBinaryImagePtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorGetBinaryImage");
        if (pageIteratorGetBinaryImagePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorGetBinaryImage not found in the library");
        _tessPageIteratorGetBinaryImage =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint>)pageIteratorGetBinaryImagePtr;

        var pageIteratorGetImagePtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorGetImage");
        if (pageIteratorGetImagePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorGetImage not found in the library");
        _tessPageIteratorGetImage =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int, nint, int*, int*, nint>)pageIteratorGetImagePtr;

        var pageIteratorBaselinePtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorBaseline");
        if (pageIteratorBaselinePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorBaseline not found in the library");
        _tessPageIteratorBaseline =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>)pageIteratorBaselinePtr;

        var pageIteratorOrientationPtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorOrientation");
        if (pageIteratorOrientationPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorOrientation not found in the library");
        _tessPageIteratorOrientation =
            (delegate* unmanaged[Cdecl]<nint, OrientationPage*, WritingDirection*, TextlineOrder*, float*, void>)
            pageIteratorOrientationPtr;

        var pageIteratorParagraphInfoPtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorParagraphInfo");
        if (pageIteratorParagraphInfoPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorParagraphInfo not found in the library");
        _tessPageIteratorParagraphInfo =
            (delegate* unmanaged[Cdecl]<nint, ParagraphJustification*, byte*, byte*, int*, void>)
            pageIteratorParagraphInfoPtr;

        var pageIteratorGetWordFontAttributesPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorGetWordFontAttributes");
        if (pageIteratorGetWordFontAttributesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorGetWordFontAttributes not found in the library");
        _tessPageIteratorGetWordFontAttributes =
            (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte*, byte*, byte*, byte*, int*, int*, byte>)
            pageIteratorGetWordFontAttributesPtr;

        var pageIteratorCopyPtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorCopy");
        if (pageIteratorCopyPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorCopy not found in the library");
        _tessPageIteratorCopy = (delegate* unmanaged[Cdecl]<nint, nint>)pageIteratorCopyPtr;

        var pageIteratorDeletePtr = NativeLibrary.GetExport(_libraryHandle, "TessPageIteratorDelete");
        if (pageIteratorDeletePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPageIteratorDelete not found in the library");
        _tessPageIteratorDelete = (delegate* unmanaged[Cdecl]<nint, void>)pageIteratorDeletePtr;

        // ResultIterator
        var getIteratorPtr = NativeLibrary.GetExport(_libraryHandle, "TessBaseAPIGetIterator");
        if (getIteratorPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBaseAPIGetIterator not found in the library");
        _tessBaseApiGetIterator = (delegate* unmanaged[Cdecl]<nint, nint>)getIteratorPtr;

        var resultIteratorCopyPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorCopy");
        if (resultIteratorCopyPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorCopy not found in the library");
        _tessResultIteratorCopy = (delegate* unmanaged[Cdecl]<nint, nint>)resultIteratorCopyPtr;

        var resultIteratorDeletePtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorDelete");
        if (resultIteratorDeletePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorDelete not found in the library");
        _tessResultIteratorDelete = (delegate* unmanaged[Cdecl]<nint, void>)resultIteratorDeletePtr;

        var resultIteratorNextPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorNext");
        if (resultIteratorNextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorNext not found in the library");
        _tessResultIteratorNext = (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, byte>)resultIteratorNextPtr;

        var resultIteratorIsAtFinalElementPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorIsAtFinalElement");
        if (resultIteratorIsAtFinalElementPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorIsAtFinalElement not found in the library");
        _tessResultIteratorIsAtFinalElement =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, PageIteratorLevel, byte>)
            resultIteratorIsAtFinalElementPtr;

        var resultIteratorGetUtf8TextPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetUTF8Text");
        if (resultIteratorGetUtf8TextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetUTF8Text not found in the library");
        _tessResultIteratorGetUtf8Text =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint>)resultIteratorGetUtf8TextPtr;

        var resultIteratorGetConfidencePtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetConfidence");
        if (resultIteratorGetConfidencePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetConfidence not found in the library");
        _tessResultIteratorGetConfidence =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, float>)resultIteratorGetConfidencePtr;

        var resultIteratorWordConfidencesPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorWordConfidences");
        if (resultIteratorWordConfidencesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorWordConfidences not found in the library");
        _tessResultIteratorWordConfidences = (delegate* unmanaged[Cdecl]<nint, nint>)resultIteratorWordConfidencesPtr;

        var resultIteratorWordRecognitionLanguagePtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorWordRecognitionLanguage");
        if (resultIteratorWordRecognitionLanguagePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorWordRecognitionLanguage not found in the library");
        _tessResultIteratorWordRecognitionLanguage =
            (delegate* unmanaged[Cdecl]<nint, nint>)resultIteratorWordRecognitionLanguagePtr;

        var resultIteratorWordFontAttributesPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorWordFontAttributes");
        if (resultIteratorWordFontAttributesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorWordFontAttributes not found in the library");
        _tessResultIteratorWordFontAttributes =
            (delegate* unmanaged[Cdecl]<nint, byte*, byte*, byte*, byte*, byte*, byte*, int*, int*, byte>)
            resultIteratorWordFontAttributesPtr;

        var resultIteratorWordIsFromDictionaryPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorWordIsFromDictionary");
        if (resultIteratorWordIsFromDictionaryPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorWordIsFromDictionary not found in the library");
        _tessResultIteratorWordIsFromDictionary =
            (delegate* unmanaged[Cdecl]<nint, byte>)resultIteratorWordIsFromDictionaryPtr;

        var resultIteratorWordIsNumericPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorWordIsNumeric");
        if (resultIteratorWordIsNumericPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorWordIsNumeric not found in the library");
        _tessResultIteratorWordIsNumeric = (delegate* unmanaged[Cdecl]<nint, byte>)resultIteratorWordIsNumericPtr;

        var resultIteratorSymbolIsSuperscriptPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorSymbolIsSuperscript");
        if (resultIteratorSymbolIsSuperscriptPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorSymbolIsSuperscript not found in the library");
        _tessResultIteratorSymbolIsSuperscript =
            (delegate* unmanaged[Cdecl]<nint, byte>)resultIteratorSymbolIsSuperscriptPtr;

        var resultIteratorSymbolIsSubscriptPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorSymbolIsSubscript");
        if (resultIteratorSymbolIsSubscriptPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorSymbolIsSubscript not found in the library");
        _tessResultIteratorSymbolIsSubscript =
            (delegate* unmanaged[Cdecl]<nint, byte>)resultIteratorSymbolIsSubscriptPtr;

        var resultIteratorSymbolIsDropcapPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorSymbolIsDropcap");
        if (resultIteratorSymbolIsDropcapPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorSymbolIsDropcap not found in the library");
        _tessResultIteratorSymbolIsDropcap = (delegate* unmanaged[Cdecl]<nint, byte>)resultIteratorSymbolIsDropcapPtr;

        var resultIteratorGetWordStrAttributesPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetWordStrAttributes");
        if (resultIteratorGetWordStrAttributesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetWordStrAttributes not found in the library");
        _tessResultIteratorGetWordStrAttributes =
            (delegate* unmanaged[Cdecl]<nint, int*, int*, int*, int*, int*, int*, int*, int*, nint>)
            resultIteratorGetWordStrAttributesPtr;

        var resultIteratorGetWordLstmChoicePtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetWordLSTMChoice");
        if (resultIteratorGetWordLstmChoicePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetWordLSTMChoice not found in the library");
        _tessResultIteratorGetWordLstmChoice =
            (delegate* unmanaged[Cdecl]<nint, nint>)resultIteratorGetWordLstmChoicePtr;

        var resultIteratorGetWordTimestepPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetWordTimestep");
        if (resultIteratorGetWordTimestepPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetWordTimestep not found in the library");
        _tessResultIteratorGetWordTimestep = (delegate* unmanaged[Cdecl]<nint, nint>)resultIteratorGetWordTimestepPtr;

        var resultIteratorGetPageIteratorPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetPageIterator");
        if (resultIteratorGetPageIteratorPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetPageIterator not found in the library");
        _tessResultIteratorGetPageIterator = (delegate* unmanaged[Cdecl]<nint, nint>)resultIteratorGetPageIteratorPtr;

        var resultIteratorGetPageIteratorConstPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetPageIteratorConst");
        if (resultIteratorGetPageIteratorConstPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetPageIteratorConst not found in the library");
        _tessResultIteratorGetPageIteratorConst =
            (delegate* unmanaged[Cdecl]<nint, nint>)resultIteratorGetPageIteratorConstPtr;

        var resultIteratorGetImagePtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetImage");
        if (resultIteratorGetImagePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetImage not found in the library");
        _tessResultIteratorGetImage =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int, nint, int*, int*, nint>)resultIteratorGetImagePtr;

        var resultIteratorBoundingBoxPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorBoundingBox");
        if (resultIteratorBoundingBoxPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorBoundingBox not found in the library");
        _tessResultIteratorBoundingBox =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>)
            resultIteratorBoundingBoxPtr;

        var resultIteratorBaselinePtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorBaseline");
        if (resultIteratorBaselinePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorBaseline not found in the library");
        _tessResultIteratorBaseline =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, int*, int*, int*, int*, byte>)
            resultIteratorBaselinePtr;

        var resultIteratorBlockTypePtr = NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorBlockType");
        if (resultIteratorBlockTypePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorBlockType not found in the library");
        _tessResultIteratorBlockType = (delegate* unmanaged[Cdecl]<nint, PolyBlockType>)resultIteratorBlockTypePtr;

        var resultIteratorGetBinaryImagePtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetBinaryImage");
        if (resultIteratorGetBinaryImagePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetBinaryImage not found in the library");
        _tessResultIteratorGetBinaryImage =
            (delegate* unmanaged[Cdecl]<nint, PageIteratorLevel, nint>)resultIteratorGetBinaryImagePtr;

        var resultIteratorGetChoiceIteratorPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultIteratorGetChoiceIterator");
        if (resultIteratorGetChoiceIteratorPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultIteratorGetChoiceIterator not found in the library");
        _tessResultIteratorGetChoiceIterator =
            (delegate* unmanaged[Cdecl]<nint, nint>)resultIteratorGetChoiceIteratorPtr;

        // ChoiceIterator
        var choiceIteratorDeletePtr = NativeLibrary.GetExport(_libraryHandle, "TessChoiceIteratorDelete");
        if (choiceIteratorDeletePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessChoiceIteratorDelete not found in the library");
        _tessChoiceIteratorDelete = (delegate* unmanaged[Cdecl]<nint, void>)choiceIteratorDeletePtr;

        var choiceIteratorNextPtr = NativeLibrary.GetExport(_libraryHandle, "TessChoiceIteratorNext");
        if (choiceIteratorNextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessChoiceIteratorNext not found in the library");
        _tessChoiceIteratorNext = (delegate* unmanaged[Cdecl]<nint, byte>)choiceIteratorNextPtr;

        var choiceIteratorGetUtf8TextPtr = NativeLibrary.GetExport(_libraryHandle, "TessChoiceIteratorGetUTF8Text");
        if (choiceIteratorGetUtf8TextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessChoiceIteratorGetUTF8Text not found in the library");
        _tessChoiceIteratorGetUtf8Text = (delegate* unmanaged[Cdecl]<nint, nint>)choiceIteratorGetUtf8TextPtr;

        var choiceIteratorGetConfidencePtr = NativeLibrary.GetExport(_libraryHandle, "TessChoiceIteratorGetConfidence");
        if (choiceIteratorGetConfidencePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessChoiceIteratorGetConfidence not found in the library");
        _tessChoiceIteratorGetConfidence = (delegate* unmanaged[Cdecl]<nint, float>)choiceIteratorGetConfidencePtr;

        var choiceIteratorGetWordConfidencesPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessChoiceIteratorGetWordConfidences");
        if (choiceIteratorGetWordConfidencesPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessChoiceIteratorGetWordConfidences not found in the library");
        _tessChoiceIteratorGetWordConfidences =
            (delegate* unmanaged[Cdecl]<nint, nint>)choiceIteratorGetWordConfidencesPtr;

        // Monitor
        var monitorCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessMonitorCreate");
        if (monitorCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessMonitorCreate not found in the library");
        _tessMonitorCreate = (delegate* unmanaged[Cdecl]<nint>)monitorCreatePtr;

        var monitorDeletePtr = NativeLibrary.GetExport(_libraryHandle, "TessMonitorDelete");
        if (monitorDeletePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessMonitorDelete not found in the library");
        _tessMonitorDelete = (delegate* unmanaged[Cdecl]<nint, void>)monitorDeletePtr;

        var monitorSetCancelFuncPtr = NativeLibrary.GetExport(_libraryHandle, "TessMonitorSetCancelFunc");
        if (monitorSetCancelFuncPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessMonitorSetCancelFunc not found in the library");
        _tessMonitorSetCancelFunc = (delegate* unmanaged[Cdecl]<nint, nint, void>)monitorSetCancelFuncPtr;

        var monitorGetCancelThisPtr = NativeLibrary.GetExport(_libraryHandle, "TessMonitorGetCancelThis");
        if (monitorGetCancelThisPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessMonitorGetCancelThis not found in the library");
        _tessMonitorGetCancelThis = (delegate* unmanaged[Cdecl]<nint, nint>)monitorGetCancelThisPtr;

        var monitorSetCancelThisPtr = NativeLibrary.GetExport(_libraryHandle, "TessMonitorSetCancelThis");
        if (monitorSetCancelThisPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessMonitorSetCancelThis not found in the library");
        _tessMonitorSetCancelThis = (delegate* unmanaged[Cdecl]<nint, nint, void>)monitorSetCancelThisPtr;

        var monitorGetProgressPtr = NativeLibrary.GetExport(_libraryHandle, "TessMonitorGetProgress");
        if (monitorGetProgressPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessMonitorGetProgress not found in the library");
        _tessMonitorGetProgress = (delegate* unmanaged[Cdecl]<nint, int>)monitorGetProgressPtr;

        var monitorSetProgressPtr = NativeLibrary.GetExport(_libraryHandle, "TessMonitorSetProgress");
        if (monitorSetProgressPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessMonitorSetProgress not found in the library");
        _tessMonitorSetProgress = (delegate* unmanaged[Cdecl]<nint, int, void>)monitorSetProgressPtr;

        // Renderers
        var textRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessTextRendererCreate");
        if (textRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessTextRendererCreate not found in the library");
        _tessTextRendererCreate = (delegate* unmanaged[Cdecl]<byte*, nint>)textRendererCreatePtr;

        var hOcrRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessHOcrRendererCreate");
        if (hOcrRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessHOcrRendererCreate not found in the library");
        _tessHOcrRendererCreate = (delegate* unmanaged[Cdecl]<byte*, nint>)hOcrRendererCreatePtr;

        var hOcrRendererCreate2Ptr = NativeLibrary.GetExport(_libraryHandle, "TessHOcrRendererCreate2");
        if (hOcrRendererCreate2Ptr == nint.Zero)
            throw new EntryPointNotFoundException("TessHOcrRendererCreate2 not found in the library");
        _tessHOcrRendererCreate2 = (delegate* unmanaged[Cdecl]<byte*, byte, nint>)hOcrRendererCreate2Ptr;

        var altoRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessAltoRendererCreate");
        if (altoRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessAltoRendererCreate not found in the library");
        _tessAltoRendererCreate = (delegate* unmanaged[Cdecl]<byte*, nint>)altoRendererCreatePtr;

        var tsvRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessTsvRendererCreate");
        if (tsvRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessTsvRendererCreate not found in the library");
        _tessTsvRendererCreate = (delegate* unmanaged[Cdecl]<byte*, nint>)tsvRendererCreatePtr;

        var pdfRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessPDFRendererCreate");
        if (pdfRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessPDFRendererCreate not found in the library");
        _tessPdfRendererCreate = (delegate* unmanaged[Cdecl]<byte*, byte*, byte, nint>)pdfRendererCreatePtr;

        var unlvRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessUnlvRendererCreate");
        if (unlvRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessUnlvRendererCreate not found in the library");
        _tessUnlvRendererCreate = (delegate* unmanaged[Cdecl]<byte*, nint>)unlvRendererCreatePtr;

        var boxTextRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessBoxTextRendererCreate");
        if (boxTextRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessBoxTextRendererCreate not found in the library");
        _tessBoxTextRendererCreate = (delegate* unmanaged[Cdecl]<byte*, nint>)boxTextRendererCreatePtr;

        var lstmBoxRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessLSTMBoxRendererCreate");
        if (lstmBoxRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessLSTMBoxRendererCreate not found in the library");
        _tessLstmBoxRendererCreate = (delegate* unmanaged[Cdecl]<byte*, nint>)lstmBoxRendererCreatePtr;

        var wordStrBoxRendererCreatePtr = NativeLibrary.GetExport(_libraryHandle, "TessWordStrBoxRendererCreate");
        if (wordStrBoxRendererCreatePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessWordStrBoxRendererCreate not found in the library");
        _tessWordStrBoxRendererCreate = (delegate* unmanaged[Cdecl]<byte*, nint>)wordStrBoxRendererCreatePtr;

        var deleteResultRendererPtr = NativeLibrary.GetExport(_libraryHandle, "TessDeleteResultRenderer");
        if (deleteResultRendererPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessDeleteResultRenderer not found in the library");
        _tessDeleteResultRenderer = (delegate* unmanaged[Cdecl]<nint, void>)deleteResultRendererPtr;

        var resultRendererInsertPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererInsert");
        if (resultRendererInsertPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererInsert not found in the library");
        _tessResultRendererInsert = (delegate* unmanaged[Cdecl]<nint, nint, void>)resultRendererInsertPtr;

        var resultRendererNextPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererNext");
        if (resultRendererNextPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererNext not found in the library");
        _tessResultRendererNext = (delegate* unmanaged[Cdecl]<nint, nint>)resultRendererNextPtr;

        var resultRendererBeginDocumentPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererBeginDocument");
        if (resultRendererBeginDocumentPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererBeginDocument not found in the library");
        _tessResultRendererBeginDocument =
            (delegate* unmanaged[Cdecl]<nint, byte*, byte>)resultRendererBeginDocumentPtr;

        var resultRendererAddImagePtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererAddImage");
        if (resultRendererAddImagePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererAddImage not found in the library");
        _tessResultRendererAddImage = (delegate* unmanaged[Cdecl]<nint, nint, byte>)resultRendererAddImagePtr;

        var resultRendererEndDocumentPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererEndDocument");
        if (resultRendererEndDocumentPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererEndDocument not found in the library");
        _tessResultRendererEndDocument = (delegate* unmanaged[Cdecl]<nint, byte>)resultRendererEndDocumentPtr;

        var resultRendererExtentionPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererExtention");
        if (resultRendererExtentionPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererExtention not found in the library");
        _tessResultRendererExtention = (delegate* unmanaged[Cdecl]<nint, nint>)resultRendererExtentionPtr;

        var resultRendererTitlePtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererTitle");
        if (resultRendererTitlePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererTitle not found in the library");
        _tessResultRendererTitle = (delegate* unmanaged[Cdecl]<nint, nint>)resultRendererTitlePtr;

        var resultRendererImageNumPtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererImageNum");
        if (resultRendererImageNumPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererImageNum not found in the library");
        _tessResultRendererImageNum = (delegate* unmanaged[Cdecl]<nint, int>)resultRendererImageNumPtr;

        var resultRendererOutputTypePtr = NativeLibrary.GetExport(_libraryHandle, "TessResultRendererOutputType");
        if (resultRendererOutputTypePtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererOutputType not found in the library");
        _tessResultRendererOutputType = (delegate* unmanaged[Cdecl]<nint, nint>)resultRendererOutputTypePtr;

        var resultRendererSetPermissionsPtr =
            NativeLibrary.GetExport(_libraryHandle, "TessResultRendererSetPermissions");
        if (resultRendererSetPermissionsPtr == nint.Zero)
            throw new EntryPointNotFoundException("TessResultRendererSetPermissions not found in the library");
        _tessResultRendererSetPermissions =
            (delegate* unmanaged[Cdecl]<nint, int, void>)resultRendererSetPermissionsPtr;
    }

    // Version
    public nint TessVersion() => _tessVersion();
    public nint TessBaseApiVersion(nint handle) => _tessBaseApiVersion(handle);

// BaseAPI Lifecycle
    public nint TessBaseApiCreate() => _tessBaseApiCreate();
    public void TessBaseApiDelete(nint handle) => _tessBaseApiDelete(handle);

// BaseAPI Initialization
    public int TessBaseApiInit3(nint handle, byte* dataPath, byte* language) =>
        _tessBaseApiInit3(handle, dataPath, language);

    public int TessBaseApiInit4(nint handle, byte* dataPath, byte* language, int mode, nint configs, int configsSize,
        nint varsVec, nint varsValues, nint varsVecSize, byte setOnlyNonDebugParams) => _tessBaseApiInit4(handle,
        dataPath, language, mode, configs, configsSize, varsVec, varsValues, varsVecSize, setOnlyNonDebugParams);

    public int TessBaseApiInit5(nint handle, byte* dataPath, byte* language, int mode, nint configs, int configsSize,
        nint varsVec, nint varsValues, nint varsVecSize, byte setOnlyNonDebugParams) => _tessBaseApiInit5(handle,
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

    public void TessBaseApiSetWarningHandler(nint handle, nint warningHandler) =>
        _tessBaseApiSetWarningHandler(handle, warningHandler);

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
    public nint TessBaseApiGetOutputName(nint handle) => _tessBaseApiGetOutputName(handle);

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
    public nint TessBaseApiGetHOCRText(nint handle, int pageNumber) => _tessBaseApiGetHocrText(handle, pageNumber);
    public nint TessBaseApiGetAltoText(nint handle, int pageNumber) => _tessBaseApiGetAltoText(handle, pageNumber);
    public nint TessBaseApiGetTsvText(nint handle, int pageNumber) => _tessBaseApiGetTsvText(handle, pageNumber);

    public nint TessBaseApiGetLSTMBoxText(nint handle, int pageNumber) =>
        _tessBaseApiGetLstmBoxText(handle, pageNumber);

    public nint TessBaseApiGetWordStrBoxText(nint handle, int pageNumber) =>
        _tessBaseApiGetWordStrBoxText(handle, pageNumber);

    public nint TessBaseApiGetUNLVText(nint handle) => _tessBaseApiGetUnlvText(handle);
    public nint TessBaseApiGetOsdText(nint handle, int pageNumber) => _tessBaseApiGetOsdText(handle, pageNumber);

    public nint TessBaseApiGetBestLSTMBoxText(nint handle, int pageNumber) =>
        _tessBaseApiGetBestLstmBoxText(handle, pageNumber);

// BaseAPI Text Deletion
    public void TessDeleteText(nint text) => _tessDeleteText(text);
    public void TessDeleteTextArray(nint arr) => _tessDeleteTextArray(arr);
    public void TessDeleteIntArray(nint arr) => _tessDeleteIntArray(arr);
    public void TessDeleteBlockList(nint blockList) => _tessDeleteBlockList(blockList);

// BaseAPI Confidence
    public int TessBaseApiMeanTextConf(nint handle) => _tessBaseApiMeanTextConf(handle);
    public nint TessBaseApiAllWordConfidences(nint handle) => _tessBaseApiAllWordConfidences(handle);

// BaseAPI Analysis
    public int TessBaseApiAnalyseLayout(nint handle) => _tessBaseApiAnalyseLayout(handle);

    public byte TessBaseApiDetectOrientationScript(nint handle, int* orientDeg, float* orientConf, byte** scriptName,
        float* scriptConf) =>
        _tessBaseApiDetectOrientationScript(handle, orientDeg, orientConf, scriptName, scriptConf);

    public nint TessBaseApiDetectOs(nint handle, OrientationPage* orientation, WritingDirection* writingDirection,
        TextlineOrder* textlineOrder, float* deskewAngle) =>
        _tessBaseApiDetectOs(handle, orientation, writingDirection, textlineOrder, deskewAngle);

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

// BaseAPI LSTM
    public nint TessBaseApiGetLstmChoice(nint handle) => _tessBaseApiGetLstmChoice(handle);
    public nint TessBaseApiGetLstmTimestep(nint handle) => _tessBaseApiGetLstmTimestep(handle);

// BaseAPI Adaptive Classifier
    public void TessBaseApiSetAdaptiveClassifier(nint handle, byte enable) =>
        _tessBaseApiSetAdaptiveClassifier(handle, enable);

    public byte TessBaseApiGetAdaptiveClassifier(nint handle) => _tessBaseApiGetAdaptiveClassifier(handle);

// BaseAPI Features (Training)
    public nint TessBaseApiGetFeaturesForBlob(nint handle, nint blob, int* featureSize) =>
        _tessBaseApiGetFeaturesForBlob(handle, blob, featureSize);

    public void TessBaseApiFreeFeatures(nint handle, nint features) => _tessBaseApiFreeFeatures(handle, features);

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

    public byte TessPageIteratorGetWordFontAttributes(nint iterator, byte* isBold, byte* isItalic, byte* isUnderlined,
        byte* isMonospace, byte* isSerif, byte* isSmallCaps, int* pointSize, int* fontId) =>
        _tessPageIteratorGetWordFontAttributes(iterator, isBold, isItalic, isUnderlined, isMonospace, isSerif,
            isSmallCaps, pointSize, fontId);

    public nint TessPageIteratorCopy(nint iterator) => _tessPageIteratorCopy(iterator);
    public void TessPageIteratorDelete(nint iterator) => _tessPageIteratorDelete(iterator);

// ResultIterator
    public nint TessBaseApiGetIterator(nint handle) => _tessBaseApiGetIterator(handle);
    public nint TessResultIteratorCopy(nint iterator) => _tessResultIteratorCopy(iterator);
    public void TessResultIteratorDelete(nint iterator) => _tessResultIteratorDelete(iterator);

    public byte TessResultIteratorNext(nint iterator, PageIteratorLevel level) =>
        _tessResultIteratorNext(iterator, level);

    public byte TessResultIteratorIsAtFinalElement(nint iterator, PageIteratorLevel level, PageIteratorLevel element) =>
        _tessResultIteratorIsAtFinalElement(iterator, level, element);

    public nint TessResultIteratorGetUtf8Text(nint iterator, PageIteratorLevel level) =>
        _tessResultIteratorGetUtf8Text(iterator, level);

    public float TessResultIteratorGetConfidence(nint iterator, PageIteratorLevel level) =>
        _tessResultIteratorGetConfidence(iterator, level);

    public nint TessResultIteratorWordConfidences(nint iterator) => _tessResultIteratorWordConfidences(iterator);

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

    public nint TessResultIteratorGetWordStrAttributes(nint iterator, int* isBold, int* isItalic, int* isUnderlined,
        int* isMonospace, int* isSerif, int* isSmallCaps, int* pointSize, int* fontId) =>
        _tessResultIteratorGetWordStrAttributes(iterator, isBold, isItalic, isUnderlined, isMonospace, isSerif,
            isSmallCaps, pointSize, fontId);

    public nint TessResultIteratorGetWordLstmChoice(nint iterator) => _tessResultIteratorGetWordLstmChoice(iterator);
    public nint TessResultIteratorGetWordTimestep(nint iterator) => _tessResultIteratorGetWordTimestep(iterator);
    public nint TessResultIteratorGetPageIterator(nint iterator) => _tessResultIteratorGetPageIterator(iterator);

    public nint TessResultIteratorGetPageIteratorConst(nint iterator) =>
        _tessResultIteratorGetPageIteratorConst(iterator);

    public nint TessResultIteratorGetImage(nint iterator, PageIteratorLevel level, int padding, nint originalImage,
        int* left, int* top) => _tessResultIteratorGetImage(iterator, level, padding, originalImage, left, top);

    public byte TessResultIteratorBoundingBox(nint iterator, PageIteratorLevel level, int* left, int* top, int* right,
        int* bottom) => _tessResultIteratorBoundingBox(iterator, level, left, top, right, bottom);

    public byte TessResultIteratorBaseline(nint iterator, PageIteratorLevel level, int* x1, int* y1, int* x2,
        int* y2) => _tessResultIteratorBaseline(iterator, level, x1, y1, x2, y2);

    public PolyBlockType TessResultIteratorBlockType(nint iterator) => _tessResultIteratorBlockType(iterator);

    public nint TessResultIteratorGetBinaryImage(nint iterator, PageIteratorLevel level) =>
        _tessResultIteratorGetBinaryImage(iterator, level);

    public nint TessResultIteratorGetChoiceIterator(nint iterator) => _tessResultIteratorGetChoiceIterator(iterator);

// ChoiceIterator
    public void TessChoiceIteratorDelete(nint choiceIterator) => _tessChoiceIteratorDelete(choiceIterator);
    public byte TessChoiceIteratorNext(nint choiceIterator) => _tessChoiceIteratorNext(choiceIterator);
    public nint TessChoiceIteratorGetUtf8Text(nint choiceIterator) => _tessChoiceIteratorGetUtf8Text(choiceIterator);

    public float TessChoiceIteratorGetConfidence(nint choiceIterator) =>
        _tessChoiceIteratorGetConfidence(choiceIterator);

    public nint TessChoiceIteratorGetWordConfidences(nint choiceIterator) =>
        _tessChoiceIteratorGetWordConfidences(choiceIterator);

// Monitor
    public nint TessMonitorCreate() => _tessMonitorCreate();
    public void TessMonitorDelete(nint monitor) => _tessMonitorDelete(monitor);

    public void TessMonitorSetCancelFunc(nint monitor, nint cancelFunc) =>
        _tessMonitorSetCancelFunc(monitor, cancelFunc);

    public nint TessMonitorGetCancelThis(nint monitor) => _tessMonitorGetCancelThis(monitor);

    public void TessMonitorSetCancelThis(nint monitor, nint cancelThis) =>
        _tessMonitorSetCancelThis(monitor, cancelThis);

    public int TessMonitorGetProgress(nint monitor) => _tessMonitorGetProgress(monitor);
    public void TessMonitorSetProgress(nint monitor, int progress) => _tessMonitorSetProgress(monitor, progress);

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
    public nint TessResultRendererExtention(nint renderer) => _tessResultRendererExtention(renderer);
    public nint TessResultRendererTitle(nint renderer) => _tessResultRendererTitle(renderer);
    public int TessResultRendererImageNum(nint renderer) => _tessResultRendererImageNum(renderer);
    public nint TessResultRendererOutputType(nint renderer) => _tessResultRendererOutputType(renderer);

    public void TessResultRendererSetPermissions(nint renderer, int permissions) =>
        _tessResultRendererSetPermissions(renderer, permissions);

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