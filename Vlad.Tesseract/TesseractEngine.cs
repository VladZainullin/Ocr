using System.Runtime.InteropServices;
using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

internal sealed class TesseractEngine : IDisposable, ITesseractEngine
{
    private bool _disposed;

    public nint Handle { get; } = TesseractNative.TessBaseApiCreate();

    public static string Version => TesseractNative.TessVersion();

    public static string DataPath => TesseractNative.TessBaseApiGetDataPath();

    public PageSegmentationMode PageSegmentationMode
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiGetPageSegMode(Handle);
        }
    }

    public ITesseractResultRenderer TextRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessTextRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer HOcrRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessHOcrRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer HOcrRendererCreate(string outputName, bool fontInfo)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessHOcrRendererCreate2(outputName, fontInfo);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer AltoRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessAltoRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer TsvRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessTsvRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer PdfRendererCreate(string outputName, string dataDir, bool textOnly)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessPdfRendererCreate(outputName, dataDir, textOnly);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public IReadOnlyList<string> GetLoadedLanguages()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var listPtr = TesseractNative.TessBaseApiGetLoadedLanguagesAsVector(Handle);
        if (listPtr == nint.Zero)
        {
            TesseractNative.TessDeleteTextArray(listPtr);
            return [];
        }
        
        try
        {
            var languages = new List<string>();

            for (var index = 0;; index++)
            {
                var stringPointer = Marshal.ReadIntPtr(listPtr, index * nint.Size);
                if (stringPointer == nint.Zero) break;

                var language = Marshal.PtrToStringUTF8(stringPointer);
                if (language is not null) languages.Add(language);
            }

            return languages.AsReadOnly();
        }
        finally
        {
            TesseractNative.TessDeleteTextArray(listPtr);
        }
    }

    public IReadOnlyList<string> GetAvailableLanguages()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var listPtr = TesseractNative.TessBaseApiGetAvailableLanguagesAsVector(Handle);
        if (listPtr == nint.Zero)
        {
            TesseractNative.TessDeleteTextArray(listPtr);
            return [];
        }

        try
        {
            var languages = new List<string>();

            for (var index = 0;; index++)
            {
                var stringPointer = Marshal.ReadIntPtr(listPtr, index * nint.Size);
                if (stringPointer == nint.Zero) break;

                var language = Marshal.PtrToStringUTF8(stringPointer);
                if (language is not null) languages.Add(language);
            }

            return languages.AsReadOnly();
        }
        finally
        {
            TesseractNative.TessDeleteTextArray(listPtr);
        }
    }

    public ITesseractResultRenderer UnlvRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessUnlvRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer BoxTextRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessBoxTextRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer WordStrBoxRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessWordStrBoxRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer LstmBoxRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var rendererPtr = TesseractNative.TessLstmBoxRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public void SetVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetVariable(Handle, name, value);
    }

    public void SetDebugVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetDebugVariable(Handle, name, value);
    }

    public void SetInputName(IPix pix)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetInputImage(Handle, pix.Handle);
    }

    public string GetVariable(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetStringVariable(Handle, name);
    }

    public bool TryGetVariable(string name, out int? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (TesseractNative.TessBaseApiGetIntVariable(Handle, name, out var v))
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetVariable(string name, out double? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (TesseractNative.TessBaseApiGetDoubleVariable(Handle, name, out var v))
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetVariable(string name, out bool? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (TesseractNative.TessBaseApiGetBoolVariable(Handle, name, out var v))
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }

    public void SetInputName(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetInputName(Handle, name);
    }

    public string InputName
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiGetInputName(Handle);
        }
    }

    public string Text
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiGetUtf8Text(Handle);
        }
    }

    public float MeanTextConfidence
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiMeanTextConf(Handle);
        }
    }

    public string GetHOcrText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetHOcrText(Handle, pageNumber);
    }

    public string GetAltoText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetAltoText(Handle, pageNumber);
    }

    public string GetTsvText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetTsvText(Handle, pageNumber);
    }

    public string GetLstmText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetLstmBoxText(Handle, pageNumber);
    }

    public string GetBoxText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetBoxText(Handle, pageNumber);
    }

    public void SetSegmentationMode(PageSegmentationMode mode)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetPageSegMode(Handle, mode);
    }

    public bool TryInitialization(string dataPath, string language)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiInit3(Handle, dataPath, language) != 0;
    }

    public int GetSourceYResolution()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetSourceYResolution(Handle);
    }

    public void SetSourceResolution(int ppi)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetSourceResolution(Handle, ppi);
    }

    public bool TryInitialization(string dataPath, string language, OcrEngineMode oem)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiInit2(Handle, dataPath, language, oem) != 0;
    }

    public void SetImage(IPix image)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetImage2(Handle, image.Handle);
    }

    public bool TryRecognize(ITesseractMonitor monitor)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(monitor);
        return TesseractNative.TessBaseApiRecognize(Handle, monitor.Handle) != 0;
    }

    public void SetRectangle(int left, int top, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetRectangle(left, top, width, height);
    }

    public unsafe void SetImage(byte[] imageData, uint width, uint height, uint bytesPerPixel)
    {
        var bytesPerLine = width * bytesPerPixel;
        fixed (byte* imagePtr = imageData)
        {
            TesseractNative.TessBaseApiSetImage(Handle, (nint)imagePtr, width, height, bytesPerPixel, bytesPerLine);
        }
    }

    public string GetInitializationLanguages()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetInitLanguagesAsString(Handle);
    }

    public ITesseractResultIterator GetIterator()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var iterator = TesseractNative.TessBaseApiGetIterator(Handle);
        return new TesseractResultIterator(iterator);
    }

    public ITesseractPageIterator AnalyzeLayout()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var iterator = TesseractNative.TessBaseApiAnalyseLayout(Handle);
        return new TesseractPageIterator(iterator);
    }

    public bool TryGetTextDirection(out int outOffset, out float slope)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetTextDirection(Handle, out outOffset, out slope);
    }

    public string GetUniChar(int uniCharId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetUniChar(Handle, uniCharId);
    }

    public void SetMinimumOrientationMargin(double margin)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetMinOrientationMargin(Handle, margin);
    }

    public void EndElement()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiEnd(Handle);
    }

    public void Clear()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiClear(Handle);
    }

    public void ClearAdaptiveClassifier()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiAdaptiveClassifier(Handle);
    }

    public bool IsValidWord(string word)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiIsValidWord(Handle, word) != 0;
    }

    public IPix GetThresholdedImage()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var pixPtr = TesseractNative.TessBaseApiGetThresholdedImage(Handle);
        return new Pix(pixPtr);
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (Handle != IntPtr.Zero)
        {
            TesseractNative.TessBaseApiDelete(Handle);
        }

        _disposed = true;
    }
}