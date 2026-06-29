using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Vlad.Tesseract;

/// <summary>
/// Provides low-level P/Invoke bindings for the Tesseract OCR library (libtesseract50.dll).
/// All methods use the Cdecl calling convention.
/// Methods returning IntPtr strings must be freed with the corresponding TessDelete* function.
/// </summary>
internal static partial class Native
{
    private const string LibraryName = "tesseract50";

    #region Version

    /// <summary>
    /// Returns the version string of the Tesseract library.
    /// </summary>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the version. The caller must NOT free this pointer.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessVersion();

    /// <summary>
    /// Returns the version string of the Tesseract library associated with the given BaseAPI handle.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the version.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiVersion(nint handle);

    #endregion

    #region BaseAPI Lifecycle

    /// <summary>
    /// Creates a new instance of TessBaseAPI.
    /// </summary>
    /// <returns>Pointer to the newly created TessBaseAPI handle.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPICreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiCreate();

    /// <summary>
    /// Destroys a TessBaseAPI instance and frees all associated memory.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance to destroy.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiDelete(nint handle);

    #endregion

    #region BaseAPI Initialization

    /// <summary>
    /// Initializes the Tesseract engine with the specified data path and language string.
    /// Equivalent to Init(dataPath, language, OEM_DEFAULT).
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="dataPath">Path to the tessdata directory.</param>
    /// <param name="language">Language code string (e.g., "eng", "rus+eng").</param>
    /// <returns>0 on success, negative value on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInit3", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInit3(nint handle, string dataPath, string language);

    /// <summary>
    /// Initializes the Tesseract engine with the specified data path, language, engine mode,
    /// config files, and variables.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="dataPath">Path to the tessdata directory.</param>
    /// <param name="language">Language code string.</param>
    /// <param name="mode">OcrEngineMode value (0 = Tesseract only, 1 = LSTM only, 2 = Tesseract + LSTM, 3 = Default).</param>
    /// <param name="configs">Pointer to an array of config file name strings.</param>
    /// <param name="configsSize">Number of config file names in the array.</param>
    /// <param name="varsVec">Pointer to an array of variable name strings.</param>
    /// <param name="varsValues">Pointer to an array of variable value strings.</param>
    /// <param name="varsVecSize">Pointer to an array of integers representing the size of each variable group.</param>
    /// <param name="setOnlyNonDebugParams">If true, only non-debug parameters are set.</param>
    /// <returns>0 on success, negative value on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInit4", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInit4(nint handle, string dataPath, string language, int mode,
        nint configs, int configsSize, nint varsVec, nint varsValues, nint varsVecSize,
        [MarshalAs(UnmanagedType.Bool)] bool setOnlyNonDebugParams);

    /// <summary>
    /// Initializes the Tesseract engine with the specified data path, language, engine mode,
    /// config files, and variables. Differs from Init4 in model loading behavior.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="dataPath">Path to the tessdata directory.</param>
    /// <param name="language">Language code string.</param>
    /// <param name="mode">OcrEngineMode value.</param>
    /// <param name="configs">Pointer to an array of config file name strings.</param>
    /// <param name="configsSize">Number of config file names in the array.</param>
    /// <param name="varsVec">Pointer to an array of variable name strings.</param>
    /// <param name="varsValues">Pointer to an array of variable value strings.</param>
    /// <param name="varsVecSize">Pointer to an array of integers representing the size of each variable group.</param>
    /// <param name="setOnlyNonDebugParams">If true, only non-debug parameters are set.</param>
    /// <returns>0 on success, negative value on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIInit5", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiInit5(nint handle, string dataPath, string language, int mode,
        nint configs, int configsSize, nint varsVec, nint varsValues, nint varsVecSize,
        [MarshalAs(UnmanagedType.Bool)] bool setOnlyNonDebugParams);

    #endregion

    #region BaseAPI Configuration

    /// <summary>
    /// Sets a configuration variable for the Tesseract engine.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="name">Name of the variable (e.g., "tessedit_char_whitelist").</param>
    /// <param name="value">Value to set.</param>
    /// <returns>TRUE if the variable was successfully set, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiSetVariable(nint handle, string name, string value);

    /// <summary>
    /// Sets a debug configuration variable for the Tesseract engine.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="name">Name of the debug variable.</param>
    /// <param name="value">Value to set.</param>
    /// <returns>TRUE if the variable was successfully set, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetDebugVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiSetDebugVariable(nint handle, string name, string value);

    /// <summary>
    /// Gets the integer value of a configuration variable.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="name">Name of the variable.</param>
    /// <param name="value">Output integer value.</param>
    /// <returns>TRUE if the variable was found, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetIntVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiGetIntVariable(nint handle, string name, out int value);

    /// <summary>
    /// Gets the boolean value of a configuration variable.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="name">Name of the variable.</param>
    /// <param name="value">Output boolean value.</param>
    /// <returns>TRUE if the variable was found, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetBoolVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiGetBoolVariable(nint handle, string name,
        [MarshalAs(UnmanagedType.Bool)] out bool value);

    /// <summary>
    /// Gets the double value of a configuration variable.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="name">Name of the variable.</param>
    /// <param name="value">Output double value.</param>
    /// <returns>TRUE if the variable was found, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetDoubleVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiGetDoubleVariable(nint handle, string name, out double value);

    /// <summary>
    /// Gets the string value of a configuration variable.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="name">Name of the variable.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetStringVariable", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetStringVariable(nint handle, string name);

    /// <summary>
    /// Gets the OpenCL device description string if OpenCL is enabled.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="device">Output pointer to the device info structure (platform-specific).</param>
    /// <returns>Pointer to a null-terminated UTF-8 string describing the OpenCL device, or NULL if not available.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetOpenCLDevice", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetOpenClDevice(nint handle, out nint device);

    /// <summary>
    /// Reads a Tesseract configuration file and applies its variables.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="filename">Path to the configuration file.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIReadConfigFile", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiReadConfigFile(nint handle, string filename);

    /// <summary>
    /// Reads a Tesseract debug configuration file and applies its variables.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="filename">Path to the debug configuration file.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIReadDebugConfigFile", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiReadDebugConfigFile(nint handle, string filename);

    /// <summary>
    /// Sets a custom warning handler callback for the Tesseract engine.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="warningHandler">Function pointer to the warning handler callback.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetWarningHandler")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetWarningHandler(nint handle, nint warningHandler);

    #endregion

    #region BaseAPI Debug

    /// <summary>
    /// Prints all current configuration variables to the given file pointer.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="fp">File pointer (FILE*) to write to. Use IntPtr.Zero for stdout.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIPrintVariables")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiPrintVariables(nint handle, nint fp);

    /// <summary>
    /// Prints all current configuration variables to the specified file.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="filename">Path to the output file.</param>
    /// <returns>TRUE on success, FALSE on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIPrintVariablesToFile", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiPrintVariablesToFile(nint handle, string filename);

    #endregion

    #region BaseAPI Page Segmentation

    /// <summary>
    /// Sets the page segmentation mode for the Tesseract engine.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="mode">Page segmentation mode.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetPageSegMode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetPageSegMode(nint handle, PageSegmentMode mode);

    /// <summary>
    /// Gets the current page segmentation mode of the Tesseract engine.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Current page segmentation mode.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetPageSegMode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial PageSegmentMode TessBaseApiGetPageSegMode(nint handle);

    #endregion

    #region BaseAPI Input/Output Names

    /// <summary>
    /// Sets the input image name (used for logging/debugging purposes).
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="name">Input image name.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetInputName", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetInputName(nint handle, string name);

    /// <summary>
    /// Gets the current input image name.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the input name.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetInputName")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetInputName(nint handle);

    /// <summary>
    /// Sets the output base name for generated output files.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="name">Output base name.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetOutputName", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetOutputName(nint handle, string name);

    /// <summary>
    /// Gets the current output base name.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the output name.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetOutputName")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetOutputName(nint handle);

    #endregion

    #region BaseAPI Source Resolution

    /// <summary>
    /// Sets the source resolution in pixels per inch. This affects text size calculations.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="ppi">Resolution in pixels per inch.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetSourceResolution")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetSourceResolution(nint handle, int ppi);

    /// <summary>
    /// Gets the Y-resolution of the source image estimated from the input.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Estimated Y-resolution in pixels per inch.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetSourceYResolution")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiGetSourceYResolution(nint handle);

    #endregion

    #region BaseAPI Image Setting

    /// <summary>
    /// Sets the image for recognition from raw pixel data.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="imagedata">Pointer to raw image pixel data (8-bit grayscale or 24/32-bit RGB/A).</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="bytesPerPixel">Number of bytes per pixel (1 for grayscale, 3 for RGB, 4 for RGBA).</param>
    /// <param name="bytesPerLine">Stride of the image in bytes (width * bytesPerPixel for no padding).</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetImage(nint handle, nint imagedata, uint width, uint height,
        uint bytesPerPixel, uint bytesPerLine);

    /// <summary>
    /// Sets the image for recognition from a Leptonica Pix structure.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="pix">Pointer to a Leptonica Pix structure.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetImage2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetImage2(nint handle, nint pix);

    /// <summary>
    /// Sets the minimum orientation margin required to determine text orientation.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="margin">Minimum margin value (default is 0.0).</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetMinOrientationMargin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetMinOrientationMargin(nint handle, double margin);

    #endregion

    #region BaseAPI Recognition

    /// <summary>
    /// Performs OCR recognition on the currently set image.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="monitor">Pointer to an ETEXT_DESC progress monitor, or IntPtr.Zero for none.</param>
    /// <returns>0 on success, negative value on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIRecognize")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiRecognize(nint handle, nint monitor);

    /// <summary>
    /// Processes a multi-page image file and produces recognition results using the given renderer.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="filename">Path to the input image file.</param>
    /// <param name="retryConfig">Optional retry configuration string, or NULL.</param>
    /// <param name="timeoutMillis">Timeout in milliseconds for processing.</param>
    /// <param name="renderer">Pointer to a ResultRenderer instance.</param>
    /// <returns>TRUE on success, FALSE on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIProcessPages", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiProcessPages(nint handle, string filename, string retryConfig,
        int timeoutMillis, nint renderer);

    #endregion

    #region BaseAPI Text Output

    /// <summary>
    /// Gets the recognized text in UTF-8 format.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetUTF8Text")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseAPIGetUTF8Text(nint handle);

    /// <summary>
    /// Gets the recognized text in hOCR format (HTML-based OCR markup).
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="pageNumber">Page number (0-based).</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetHOCRText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetHOCRText(nint handle, int pageNumber);

    /// <summary>
    /// Gets the recognized text in ALTO XML format.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="pageNumber">Page number (0-based).</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetAltoText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetAltoText(nint handle, int pageNumber);

    /// <summary>
    /// Gets the recognized text in TSV (tab-separated values) format.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="pageNumber">Page number (0-based).</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetTsvText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetTsvText(nint handle, int pageNumber);

    /// <summary>
    /// Gets the recognized text with bounding box coordinates in LSTM Box format.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="pageNumber">Page number (0-based).</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetLSTMBoxText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetLSTMBoxText(nint handle, int pageNumber);

    /// <summary>
    /// Gets the recognized text with bounding box coordinates in WordStr Box format.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="pageNumber">Page number (0-based).</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetWordStrBoxText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetWordStrBoxText(nint handle, int pageNumber);

    /// <summary>
    /// Gets the recognized text in UNLV format (used by the UNLV/ISRI OCR evaluation tools).
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetUNLVText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetUNLVText(nint handle);

    /// <summary>
    /// Gets the orientation and script detection (OSD) information as a text string.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="pageNumber">Page number (0-based).</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetOsdText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetOsdText(nint handle, int pageNumber);

    /// <summary>
    /// Gets the best recognized text with bounding box coordinates using LSTM engine.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="pageNumber">Page number (0-based).</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetBestLSTMBoxText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetBestLSTMBoxText(nint handle, int pageNumber);

    #endregion

    #region BaseAPI Text Deletion

    /// <summary>
    /// Frees a text string allocated by the Tesseract library.
    /// </summary>
    /// <param name="text">Pointer to the text string to free.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessDeleteText")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteText(nint text);

    /// <summary>
    /// Frees an array of text strings allocated by the Tesseract library.
    /// </summary>
    /// <param name="arr">Pointer to the text array to free.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessDeleteTextArray")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteTextArray(nint arr);

    /// <summary>
    /// Frees an array of integers allocated by the Tesseract library.
    /// </summary>
    /// <param name="arr">Pointer to the integer array to free.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessDeleteIntArray")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteIntArray(nint arr);

    /// <summary>
    /// Frees a block list structure allocated by the Tesseract library.
    /// </summary>
    /// <param name="blockList">Pointer to the block list to free.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessDeleteBlockList")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteBlockList(nint blockList);

    #endregion

    #region BaseAPI Confidence

    /// <summary>
    /// Returns the mean confidence of the recognized text as a percentage (0-100).
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Mean confidence value (0-100).</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIMeanTextConf")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiMeanTextConf(nint handle);

    /// <summary>
    /// Returns confidence values for all recognized words.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to an array of integer confidence values (0-100). Must be freed with TessDeleteIntArray().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIAllWordConfidences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiAllWordConfidences(nint handle);

    #endregion

    #region BaseAPI Analysis

    /// <summary>
    /// Performs layout analysis on the current image without performing recognition.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>0 on success, negative value on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIAnalyseLayout")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiAnalyseLayout(nint handle);

    /// <summary>
    /// Detects the orientation and script of the current image.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="orientDeg">Output orientation in degrees (0, 90, 180, 270).</param>
    /// <param name="orientConf">Output confidence of the orientation detection.</param>
    /// <param name="scriptName">Output script name (e.g., "Latin", "Cyrillic"). Must be freed with TessDeleteText().</param>
    /// <param name="scriptConf">Output confidence of the script detection.</param>
    /// <returns>TRUE on success, FALSE on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIDetectOrientationScript",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiDetectOrientationScript(nint handle, out int orientDeg,
        out float orientConf, out string scriptName, out float scriptConf);

    /// <summary>
    /// Detects the orientation and script of the current image (extended version).
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="orientation">Output page orientation.</param>
    /// <param name="writingDirection">Output writing direction.</param>
    /// <param name="textlineOrder">Output text line order.</param>
    /// <param name="deskewAngle">Output deskew angle in radians.</param>
    /// <returns>Pointer to an OSResults structure, or NULL on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIDetectOS")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiDetectOs(nint handle, out OrientationPage orientation,
        out WritingDirection writingDirection, out TextlineOrder textlineOrder, out float deskewAngle);

    /// <summary>
    /// Gets the direction of the recognized text.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="offset">Output offset value.</param>
    /// <param name="slope">Output slope value.</param>
    /// <returns>Pointer to a null-terminated string describing the text direction.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetTextDirection")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetTextDirection(nint handle, out int offset, out float slope);

    #endregion

    #region BaseAPI Image Retrieval

    /// <summary>
    /// Gets the thresholded (binary) image used internally by Tesseract.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a Leptonica Pix structure, or NULL on failure. Must be freed with pixDestroy().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetThresholdedImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetThresholdedImage(nint handle);

    /// <summary>
    /// Gets the scale factor of the thresholded image relative to the original image.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Scale factor as a percentage.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetThresholdedImageScaleFactor")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiGetThresholdedImageScaleFactor(nint handle);

    #endregion

    #region BaseAPI Dictionary

    /// <summary>
    /// Checks if a word is valid according to the loaded dictionaries.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="word">The word to check.</param>
    /// <returns>Non-zero if the word is valid, 0 if not, -1 on error.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIIsValidWord", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessBaseApiIsValidWord(nint handle, string word);

    /// <summary>
    /// Adapts the Tesseract engine to the given word string for improved recognition.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="mode">Page segmentation mode to use for adaptation.</param>
    /// <param name="wordStr">The word string to adapt to.</param>
    /// <returns>TRUE on success, FALSE on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIAdaptToWordStr", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiAdaptToWordStr(nint handle, PageSegmentMode mode, string wordStr);

    #endregion

    #region BaseAPI Clear

    /// <summary>
    /// Clears the current recognition results and internal state, but keeps the initialized models.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIClear")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiClear(nint handle);

    /// <summary>
    /// Clears any persistent cache used by the Tesseract engine.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIClearPersistentCache")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiClearPersistentCache(nint handle);

    #endregion

    #region BaseAPI Languages

    /// <summary>
    /// Gets a list of all available languages in the tessdata directory.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a GenericVector&lt;string&gt; structure containing language codes.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetAvailableLanguagesAsVector")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetAvailableLanguagesAsVector(nint handle);

    /// <summary>
    /// Gets the UTF-8 string representation of a unichar ID.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="unicharId">The unichar ID to look up.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetUnichar")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetUnichar(nint handle, int unicharId);

    #endregion

    #region BaseAPI LSTM

    /// <summary>
    /// Gets the LSTM choice information for the last recognition.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to an LSTM choice structure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetLSTMChoice")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetLstmChoice(nint handle);

    /// <summary>
    /// Gets the LSTM timestep information for the last recognition.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to an LSTM timestep structure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetLSTMTimestep")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetLstmTimestep(nint handle);

    #endregion

    #region BaseAPI Adaptive Classifier

    /// <summary>
    /// Enables or disables the adaptive classifier.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="enable">TRUE to enable, FALSE to disable.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPISetAdaptiveClassifier")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiSetAdaptiveClassifier(nint handle,
        [MarshalAs(UnmanagedType.Bool)] bool enable);

    /// <summary>
    /// Checks if the adaptive classifier is currently enabled.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>TRUE if enabled, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetAdaptiveClassifier")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessBaseApiGetAdaptiveClassifier(nint handle);

    #endregion

    #region BaseAPI Features (Training)

    /// <summary>
    /// Extracts feature vectors for a given blob. Used for training purposes.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="blob">Pointer to a blob structure.</param>
    /// <param name="featureSize">Output size of the feature vector.</param>
    /// <returns>Pointer to the feature vector array. Must be freed with TessBaseApiFreeFeatures().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetFeaturesForBlob")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetFeaturesForBlob(nint handle, nint blob, out int featureSize);

    /// <summary>
    /// Frees feature vectors allocated by TessBaseAPIGetFeaturesForBlob().
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <param name="features">Pointer to the feature vector array to free.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIFreeFeatures")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessBaseApiFreeFeatures(nint handle, nint features);

    #endregion

    #region PageIterator

    /// <summary>
    /// Moves the iterator to the beginning of the page.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorBegin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorBegin(nint iterator);

    /// <summary>
    /// Moves the iterator to the next element at the specified level.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="level">Page iterator level to advance.</param>
    /// <returns>TRUE if the iterator was advanced, FALSE if there are no more elements.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorNext(nint iterator, PageIteratorLevel level);

    /// <summary>
    /// Checks if the iterator is at the beginning of the specified level.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="level">Page iterator level to check.</param>
    /// <returns>TRUE if at the beginning, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorIsAtBeginningOf(nint iterator, PageIteratorLevel level);

    /// <summary>
    /// Checks if the iterator is positioned at the final element at the specified level and element.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="level">Page iterator level to check.</param>
    /// <param name="element">Page iterator level of the element to check.</param>
    /// <returns>Non-zero if at the final element, 0 otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorIsAtFinalElement")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessPageIteratorIsAtFinalElement(nint iterator, PageIteratorLevel level,
        PageIteratorLevel element);

    /// <summary>
    /// Gets the bounding box of the current element at the specified level.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <param name="left">Output left coordinate of the bounding box.</param>
    /// <param name="top">Output top coordinate of the bounding box.</param>
    /// <param name="right">Output right coordinate of the bounding box.</param>
    /// <param name="bottom">Output bottom coordinate of the bounding box.</param>
    /// <returns>TRUE if the bounding box is valid, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorBoundingBox")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorBoundingBox(nint iterator, PageIteratorLevel level,
        out int left, out int top, out int right, out int bottom);

    /// <summary>
    /// Gets the block type of the current element.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <returns>PolyBlockType value indicating the type of the current block.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorBlockType")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial PolyBlockType TessPageIteratorBlockType(nint iterator);

    /// <summary>
    /// Gets the binary image of the current element at the specified level.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <returns>Pointer to a Leptonica Pix structure, or NULL on failure. Must be freed with pixDestroy().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorGetBinaryImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessPageIteratorGetBinaryImage(nint iterator, PageIteratorLevel level);

    /// <summary>
    /// Gets the image of the current element at the specified level with optional padding.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <param name="padding">Padding in pixels to add around the element.</param>
    /// <param name="originalImage">Pointer to the original image as a Leptonica Pix.</param>
    /// <param name="left">Output left coordinate of the extracted image.</param>
    /// <param name="top">Output top coordinate of the extracted image.</param>
    /// <returns>Pointer to a Leptonica Pix structure, or NULL on failure. Must be freed with pixDestroy().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorGetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessPageIteratorGetImage(nint iterator, PageIteratorLevel level, int padding,
        nint originalImage, out int left, out int top);

    /// <summary>
    /// Gets the baseline coordinates of the current element at the specified level.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <param name="x1">Output x coordinate of the baseline start.</param>
    /// <param name="y1">Output y coordinate of the baseline start.</param>
    /// <param name="x2">Output x coordinate of the baseline end.</param>
    /// <param name="y2">Output y coordinate of the baseline end.</param>
    /// <returns>TRUE if the baseline is valid, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorBaseline")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorBaseline(nint iterator, PageIteratorLevel level, out int x1,
        out int y1, out int x2, out int y2);

    /// <summary>
    /// Gets the orientation, writing direction, text line order, and deskew angle for the current element.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="orientation">Output page orientation.</param>
    /// <param name="writingDirection">Output writing direction.</param>
    /// <param name="textlineOrder">Output text line order.</param>
    /// <param name="deskewAngle">Output deskew angle in radians.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorOrientation")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorOrientation(
        nint iterator,
        out OrientationPage orientation,
        out WritingDirection writingDirection,
        out TextlineOrder textlineOrder,
        out float deskewAngle);

    /// <summary>
    /// Gets paragraph information for the current element.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="justification">Output paragraph justification.</param>
    /// <param name="isListItem">Output indicates if the paragraph is a list item.</param>
    /// <param name="isCrown">Output indicates if the paragraph is a crown (first line of a paragraph).</param>
    /// <param name="firstLineIndent">Output first line indentation in pixels.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorParagraphInfo")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorParagraphInfo(
        nint iterator,
        out ParagraphJustification justification,
        [MarshalAs(UnmanagedType.Bool)] out bool isListItem,
        [MarshalAs(UnmanagedType.Bool)] out bool isCrown,
        out int firstLineIndent);

    /// <summary>
    /// Gets font attributes for the word at the current iterator position.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <param name="isBold">Output indicates if the font is bold.</param>
    /// <param name="isItalic">Output indicates if the font is italic.</param>
    /// <param name="isUnderlined">Output indicates if the font is underlined.</param>
    /// <param name="isMonospace">Output indicates if the font is monospace.</param>
    /// <param name="isSerif">Output indicates if the font is serif.</param>
    /// <param name="isSmallCaps">Output indicates if the font is small caps.</param>
    /// <param name="pointSize">Output font point size.</param>
    /// <param name="fontId">Output font ID.</param>
    /// <returns>TRUE if the attributes were successfully retrieved, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorGetWordFontAttributes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessPageIteratorGetWordFontAttributes(
        nint iterator,
        [MarshalAs(UnmanagedType.Bool)] out bool isBold,
        [MarshalAs(UnmanagedType.Bool)] out bool isItalic,
        [MarshalAs(UnmanagedType.Bool)] out bool isUnderlined,
        [MarshalAs(UnmanagedType.Bool)] out bool isMonospace,
        [MarshalAs(UnmanagedType.Bool)] out bool isSerif,
        [MarshalAs(UnmanagedType.Bool)] out bool isSmallCaps,
        out int pointSize,
        out int fontId);

    /// <summary>
    /// Creates a copy of the page iterator.
    /// </summary>
    /// <param name="iterator">Pointer to a PageIterator instance.</param>
    /// <returns>Pointer to a new PageIterator instance. Must be freed with TessPageIteratorDelete().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorCopy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessPageIteratorCopy(nint iterator);

    /// <summary>
    /// Destroys a page iterator and frees all associated memory.
    /// </summary>
    /// <param name="iterator">Pointer to the PageIterator instance to destroy.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessPageIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessPageIteratorDelete(nint iterator);

    #endregion

    #region ResultIterator

    /// <summary>
    /// Gets a ResultIterator from the BaseAPI handle for traversing recognition results.
    /// The iterator starts at the beginning of the page.
    /// </summary>
    /// <param name="handle">Pointer to the TessBaseAPI instance.</param>
    /// <returns>Pointer to a ResultIterator instance. Must be freed with TessResultIteratorDelete().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBaseAPIGetIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBaseApiGetIterator(nint handle);

    /// <summary>
    /// Creates a copy of the result iterator.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>Pointer to a new ResultIterator instance. Must be freed with TessResultIteratorDelete().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorCopy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorCopy(nint iterator);

    /// <summary>
    /// Destroys a result iterator and frees all associated memory.
    /// </summary>
    /// <param name="iterator">Pointer to the ResultIterator instance to destroy.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessResultIteratorDelete(nint iterator);

    /// <summary>
    /// Moves the result iterator to the next element at the specified level.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="level">Page iterator level to advance.</param>
    /// <returns>TRUE if the iterator was advanced, FALSE if there are no more elements.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorNext(nint iterator, PageIteratorLevel level);

    /// <summary>
    /// Checks if the result iterator is positioned at the final element at the specified level and element.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="level">Page iterator level to check.</param>
    /// <param name="element">Page iterator level of the element to check.</param>
    /// <returns>TRUE if at the final element, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorIsAtFinalElement")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorIsAtFinalElement(nint iterator, PageIteratorLevel level,
        PageIteratorLevel element);

    /// <summary>
    /// Gets the UTF-8 recognized text for the current element at the specified level.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetUTF8Text")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetUtf8Text(nint iterator, PageIteratorLevel level);

    /// <summary>
    /// Gets the confidence value for the current element at the specified level.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <returns>Confidence value (0.0f to 100.0f).</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetConfidence")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float TessResultIteratorGetConfidence(nint iterator, PageIteratorLevel level);

    /// <summary>
    /// Gets the confidence values for each character in the current word.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>Pointer to an array of integer confidence values (0-100). Must be freed with TessDeleteIntArray().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordConfidences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorWordConfidences(nint iterator);

    /// <summary>
    /// Gets the recognition language of the current word.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the language code. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordRecognitionLanguage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorWordRecognitionLanguage(nint iterator);

    /// <summary>
    /// Gets font attributes for the current word.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="isBold">Output indicates if the font is bold.</param>
    /// <param name="isItalic">Output indicates if the font is italic.</param>
    /// <param name="isUnderlined">Output indicates if the font is underlined.</param>
    /// <param name="isMonospace">Output indicates if the font is monospace.</param>
    /// <param name="isSerif">Output indicates if the font is serif.</param>
    /// <param name="isSmallCaps">Output indicates if the font is small caps.</param>
    /// <param name="pointSize">Output font point size.</param>
    /// <param name="fontId">Output font ID.</param>
    /// <returns>TRUE if the attributes were successfully retrieved, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordFontAttributes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorWordFontAttributes(
        nint iterator,
        [MarshalAs(UnmanagedType.Bool)] out bool isBold,
        [MarshalAs(UnmanagedType.Bool)] out bool isItalic,
        [MarshalAs(UnmanagedType.Bool)] out bool isUnderlined,
        [MarshalAs(UnmanagedType.Bool)] out bool isMonospace,
        [MarshalAs(UnmanagedType.Bool)] out bool isSerif,
        [MarshalAs(UnmanagedType.Bool)] out bool isSmallCaps,
        out int pointSize,
        out int fontId);

    /// <summary>
    /// Checks if the current word is from a dictionary.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>TRUE if the word is from the dictionary, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordIsFromDictionary")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorWordIsFromDictionary(nint iterator);

    /// <summary>
    /// Checks if the current word is numeric.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>TRUE if the word is numeric, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorWordIsNumeric")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorWordIsNumeric(nint iterator);

    /// <summary>
    /// Checks if the current symbol is superscript.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>TRUE if the symbol is superscript, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorSymbolIsSuperscript")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorSymbolIsSuperscript(nint iterator);

    /// <summary>
    /// Checks if the current symbol is subscript.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>TRUE if the symbol is subscript, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorSymbolIsSubscript")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorSymbolIsSubscript(nint iterator);

    /// <summary>
    /// Checks if the current symbol is a drop cap (large initial letter).
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>TRUE if the symbol is a drop cap, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorSymbolIsDropcap")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorSymbolIsDropcap(nint iterator);

    /// <summary>
    /// Gets string-based font attributes for the current word.
    /// This method is deprecated in favor of TessResultIteratorWordFontAttributes.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="isBold">Output indicates if the font is bold (as integer).</param>
    /// <param name="isItalic">Output indicates if the font is italic (as integer).</param>
    /// <param name="isUnderlined">Output indicates if the font is underlined (as integer).</param>
    /// <param name="isMonospace">Output indicates if the font is monospace (as integer).</param>
    /// <param name="isSerif">Output indicates if the font is serif (as integer).</param>
    /// <param name="isSmallCaps">Output indicates if the font is small caps (as integer).</param>
    /// <param name="pointSize">Output font point size.</param>
    /// <param name="fontId">Output font ID.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the font name. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetWordStrAttributes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetWordStrAttributes(nint iterator,
        out int isBold, out int isItalic, out int isUnderlined, out int isMonospace, out int isSerif,
        out int isSmallCaps, out int pointSize, out int fontId);

    /// <summary>
    /// Gets the LSTM choice information for the current word.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>Pointer to an LSTM choice structure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetWordLSTMChoice")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetWordLstmChoice(nint iterator);

    /// <summary>
    /// Gets the LSTM timestep information for the current word.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>Pointer to an LSTM timestep structure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetWordTimestep")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetWordTimestep(nint iterator);

    /// <summary>
    /// Gets the underlying PageIterator from a ResultIterator.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>Pointer to a PageIterator instance. Do NOT delete separately; it is owned by the ResultIterator.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetPageIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetPageIterator(nint iterator);

    /// <summary>
    /// Gets the underlying const PageIterator from a ResultIterator.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>Pointer to a const PageIterator instance. Do NOT delete separately.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetPageIteratorConst")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetPageIteratorConst(nint iterator);

    /// <summary>
    /// Gets the image of the current element from the result iterator.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <param name="padding">Padding in pixels to add around the element.</param>
    /// <param name="originalImage">Pointer to the original image as a Leptonica Pix.</param>
    /// <param name="left">Output left coordinate of the extracted image.</param>
    /// <param name="top">Output top coordinate of the extracted image.</param>
    /// <returns>Pointer to a Leptonica Pix structure. Must be freed with pixDestroy().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetImage(nint iterator, PageIteratorLevel level, int padding,
        nint originalImage, out int left, out int top);

    /// <summary>
    /// Gets the bounding box of the current element from the result iterator.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <param name="left">Output left coordinate of the bounding box.</param>
    /// <param name="top">Output top coordinate of the bounding box.</param>
    /// <param name="right">Output right coordinate of the bounding box.</param>
    /// <param name="bottom">Output bottom coordinate of the bounding box.</param>
    /// <returns>TRUE if the bounding box is valid, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorBoundingBox")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorBoundingBox(nint iterator, PageIteratorLevel level,
        out int left, out int top, out int right, out int bottom);

    /// <summary>
    /// Gets the baseline of the current element from the result iterator.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <param name="x1">Output x coordinate of the baseline start.</param>
    /// <param name="y1">Output y coordinate of the baseline start.</param>
    /// <param name="x2">Output x coordinate of the baseline end.</param>
    /// <param name="y2">Output y coordinate of the baseline end.</param>
    /// <returns>TRUE if the baseline is valid, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorBaseline")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultIteratorBaseline(nint iterator, PageIteratorLevel level,
        out int x1, out int y1, out int x2, out int y2);

    /// <summary>
    /// Gets the block type of the current element from the result iterator.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <returns>PolyBlockType value indicating the type of the current block.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorBlockType")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial PolyBlockType TessResultIteratorBlockType(nint iterator);

    /// <summary>
    /// Gets the binary image of the current element from the result iterator.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance.</param>
    /// <param name="level">Page iterator level of the element.</param>
    /// <returns>Pointer to a Leptonica Pix structure. Must be freed with pixDestroy().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetBinaryImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetBinaryImage(nint iterator, PageIteratorLevel level);

    /// <summary>
    /// Gets a ChoiceIterator for the current symbol, allowing iteration through alternative recognition choices.
    /// </summary>
    /// <param name="iterator">Pointer to a ResultIterator instance (must be at symbol level).</param>
    /// <returns>Pointer to a ChoiceIterator instance. Must be freed with TessChoiceIteratorDelete().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultIteratorGetChoiceIterator")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultIteratorGetChoiceIterator(nint iterator);

    #endregion

    #region ChoiceIterator

    /// <summary>
    /// Destroys a choice iterator and frees all associated memory.
    /// </summary>
    /// <param name="choiceIterator">Pointer to the ChoiceIterator instance to destroy.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessChoiceIteratorDelete(nint choiceIterator);

    /// <summary>
    /// Moves the choice iterator to the next alternative recognition choice.
    /// </summary>
    /// <param name="choiceIterator">Pointer to a ChoiceIterator instance.</param>
    /// <returns>TRUE if there is another choice, FALSE otherwise.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessChoiceIteratorNext(nint choiceIterator);

    /// <summary>
    /// Gets the UTF-8 text of the current recognition choice.
    /// </summary>
    /// <param name="choiceIterator">Pointer to a ChoiceIterator instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string. Must be freed with TessDeleteText().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorGetUTF8Text")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessChoiceIteratorGetUtf8Text(nint choiceIterator);

    /// <summary>
    /// Gets the confidence value of the current recognition choice.
    /// </summary>
    /// <param name="choiceIterator">Pointer to a ChoiceIterator instance.</param>
    /// <returns>Confidence value (0.0f to 100.0f).</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorGetConfidence")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float TessChoiceIteratorGetConfidence(nint choiceIterator);

    /// <summary>
    /// Gets the confidence values for each character in the current word choice.
    /// </summary>
    /// <param name="choiceIterator">Pointer to a ChoiceIterator instance.</param>
    /// <returns>Pointer to an array of integer confidence values (0-100). Must be freed with TessDeleteIntArray().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessChoiceIteratorGetWordConfidences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessChoiceIteratorGetWordConfidences(nint choiceIterator);

    #endregion

    #region Monitor

    /// <summary>
    /// Creates a new progress monitor for tracking OCR progress.
    /// </summary>
    /// <returns>Pointer to a new ETEXT_DESC monitor structure. Must be freed with TessMonitorDelete().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorCreate")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessMonitorCreate();

    /// <summary>
    /// Destroys a progress monitor and frees associated memory.
    /// </summary>
    /// <param name="monitor">Pointer to the monitor to destroy.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorDelete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorDelete(nint monitor);

    /// <summary>
    /// Sets a cancellation callback function for the monitor.
    /// </summary>
    /// <param name="monitor">Pointer to the monitor.</param>
    /// <param name="cancelFunc">Function pointer to the cancellation callback.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorSetCancelFunc")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetCancelFunc(nint monitor, nint cancelFunc);

    /// <summary>
    /// Gets the user data pointer associated with the cancellation callback.
    /// </summary>
    /// <param name="monitor">Pointer to the monitor.</param>
    /// <returns>User data pointer passed to the cancellation callback.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorGetCancelThis")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessMonitorGetCancelThis(nint monitor);

    /// <summary>
    /// Sets the user data pointer for the cancellation callback.
    /// </summary>
    /// <param name="monitor">Pointer to the monitor.</param>
    /// <param name="cancelThis">User data pointer to pass to the cancellation callback.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorSetCancelThis")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetCancelThis(nint monitor, nint cancelThis);

    /// <summary>
    /// Gets the current progress value from the monitor.
    /// </summary>
    /// <param name="monitor">Pointer to the monitor.</param>
    /// <returns>Progress value (0-100).</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorGetProgress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessMonitorGetProgress(nint monitor);

    /// <summary>
    /// Sets the current progress value in the monitor.
    /// </summary>
    /// <param name="monitor">Pointer to the monitor.</param>
    /// <param name="progress">Progress value (0-100).</param>
    [LibraryImport(LibraryName, EntryPoint = "TessMonitorSetProgress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessMonitorSetProgress(nint monitor, int progress);

    #endregion

    #region Renderers

    /// <summary>
    /// Creates a renderer for plain text output.
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessTextRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessTextRendererCreate(string outputBase);

    /// <summary>
    /// Creates a renderer for hOCR output (HTML-based OCR markup).
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessHOcrRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessHOcrRendererCreate(string outputBase);

    /// <summary>
    /// Creates a renderer for hOCR output with optional font information.
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <param name="fontInfo">If TRUE, includes font information in the output.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessHOcrRendererCreate2", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessHOcrRendererCreate2(string outputBase,
        [MarshalAs(UnmanagedType.Bool)] bool fontInfo);

    /// <summary>
    /// Creates a renderer for ALTO XML output.
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessAltoRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessAltoRendererCreate(string outputBase);

    /// <summary>
    /// Creates a renderer for TSV (tab-separated values) output.
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessTsvRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessTsvRendererCreate(string outputBase);

    /// <summary>
    /// Creates a renderer for PDF output.
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <param name="dataDir">Path to the tessdata directory (required for PDF rendering).</param>
    /// <param name="textOnly">If TRUE, creates a text-only PDF; otherwise includes the image.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessPDFRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessPdfRendererCreate(string outputBase, string dataDir,
        [MarshalAs(UnmanagedType.Bool)] bool textOnly);

    /// <summary>
    /// Creates a renderer for UNLV format output (used by OCR evaluation tools).
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessUnlvRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessUnlvRendererCreate(string outputBase);

    /// <summary>
    /// Creates a renderer for box text output (coordinates with recognized text).
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessBoxTextRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessBoxTextRendererCreate(string outputBase);

    /// <summary>
    /// Creates a renderer for LSTM box output (coordinates with recognition using LSTM engine).
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessLSTMBoxRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessLstmBoxRendererCreate(string outputBase);

    /// <summary>
    /// Creates a renderer for WordStr box output (word-level bounding boxes with string attributes).
    /// </summary>
    /// <param name="outputBase">Base path/name for the output file.</param>
    /// <returns>Pointer to a ResultRenderer instance. Must be freed with TessDeleteResultRenderer().</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessWordStrBoxRendererCreate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessWordStrBoxRendererCreate(string outputBase);

    /// <summary>
    /// Destroys a result renderer and frees all associated memory.
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance to destroy.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessDeleteResultRenderer")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessDeleteResultRenderer(nint renderer);

    /// <summary>
    /// Inserts a sub-renderer into the renderer chain. The inserted renderer will receive results next.
    /// </summary>
    /// <param name="renderer">Pointer to the parent ResultRenderer instance.</param>
    /// <param name="subRenderer">Pointer to the sub-renderer to insert.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererInsert")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessResultRendererInsert(nint renderer, nint subRenderer);

    /// <summary>
    /// Gets the next renderer in the renderer chain.
    /// </summary>
    /// <param name="renderer">Pointer to the current ResultRenderer instance.</param>
    /// <returns>Pointer to the next ResultRenderer in the chain, or NULL if there is none.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererNext")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultRendererNext(nint renderer);

    /// <summary>
    /// Begins a new document in the renderer (required for formats like PDF that support multiple pages).
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance.</param>
    /// <param name="title">Document title.</param>
    /// <returns>TRUE on success, FALSE on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererBeginDocument", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultRendererBeginDocument(nint renderer, string title);

    /// <summary>
    /// Adds a recognized image to the document (one page of results).
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance.</param>
    /// <param name="api">Pointer to the TessBaseAPI instance with recognition results.</param>
    /// <returns>TRUE on success, FALSE on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererAddImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultRendererAddImage(nint renderer, nint api);

    /// <summary>
    /// Ends the document, finalizing the output file.
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance.</param>
    /// <returns>TRUE on success, FALSE on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererEndDocument")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TessResultRendererEndDocument(nint renderer);

    /// <summary>
    /// Gets the file extension for this renderer's output format.
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the file extension (e.g., ".pdf", ".txt").</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererExtention")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultRendererExtention(nint renderer);

    /// <summary>
    /// Gets the document title from the renderer.
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the document title.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererTitle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultRendererTitle(nint renderer);

    /// <summary>
    /// Gets the current image number (page number) being processed by the renderer.
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance.</param>
    /// <returns>Current image/page number.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererImageNum")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TessResultRendererImageNum(nint renderer);

    /// <summary>
    /// Gets a string describing the output type of this renderer (e.g., "hocr", "pdf", "txt").
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance.</param>
    /// <returns>Pointer to a null-terminated UTF-8 string containing the output type description.</returns>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererOutputType")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nint TessResultRendererOutputType(nint renderer);

    /// <summary>
    /// Sets the file permissions for the output file (mainly relevant for PDF).
    /// </summary>
    /// <param name="renderer">Pointer to the ResultRenderer instance.</param>
    /// <param name="permissions">File permission flags.</param>
    [LibraryImport(LibraryName, EntryPoint = "TessResultRendererSetPermissions")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TessResultRendererSetPermissions(nint renderer, int permissions);

    #endregion
}