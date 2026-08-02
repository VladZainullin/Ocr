using System.Runtime.InteropServices;
using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

internal sealed class TesseractEngine : IDisposable, ITesseractEngine
{
    private bool _disposed;

    public nint Handle { get; } = TesseractNative.TessBaseApiCreate();

    public static string Version => TesseractNative.TessVersion();

    public string GetDataPath() => TesseractNative.TessBaseApiGetDataPath(Handle);

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
        ArgumentNullException.ThrowIfNull(outputName);
        var rendererPtr = TesseractNative.TessUnlvRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer BoxTextRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(outputName);
        var rendererPtr = TesseractNative.TessBoxTextRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer WordStrBoxRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(outputName);
        var rendererPtr = TesseractNative.TessWordStrBoxRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public ITesseractResultRenderer LstmBoxRendererCreate(string outputName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(outputName);
        var rendererPtr = TesseractNative.TessLstmBoxRendererCreate(outputName);
        return new TesseractResultResultRenderer(rendererPtr);
    }

    public void SetVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);
        TesseractNative.TessBaseApiSetVariable(Handle, name, value);
    }

    public void SetDebugVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);
        TesseractNative.TessBaseApiSetDebugVariable(Handle, name, value);
    }

    public void SetInputName(IPix pix)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(pix);
        TesseractNative.TessBaseApiSetInputImage(Handle, pix.Handle);
    }

    public string GetVariable(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(name);
        return TesseractNative.TessBaseApiGetStringVariable(Handle, name);
    }

    public bool TryGetVariable(string name, out int? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(name);
        if (TesseractNative.TessBaseApiGetIntVariable(Handle, name, out var nativeValue))
        {
            value = nativeValue;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetVariable(string name, out double? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(name);
        if (TesseractNative.TessBaseApiGetDoubleVariable(Handle, name, out var nativeValue))
        {
            value = nativeValue;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetVariable(string name, out bool? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(name);
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
        ArgumentNullException.ThrowIfNull(name);
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

    public string? Text
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var textPtr = TesseractNative.TessBaseApiGetUtf8Text(Handle);
            try
            {
                return Marshal.PtrToStringUTF8(textPtr);
            }
            finally
            {
                TesseractNative.TessDeleteText(textPtr);
            }
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

    public string? GetHOcrText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var textPtr = TesseractNative.TessBaseApiGetHOcrText(Handle, pageNumber);
        try
        {
            return Marshal.PtrToStringUTF8(textPtr);
        }
        finally
        {
            TesseractNative.TessDeleteText(textPtr);
        }
    }

    public string? GetAltoText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var textPtr = TesseractNative.TessBaseApiGetAltoText(Handle, pageNumber);
        try
        {
            return Marshal.PtrToStringUTF8(textPtr);
        }
        finally
        {
            TesseractNative.TessDeleteText(textPtr);
        }
    }

    public string? GetTsvText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var textPtr = TesseractNative.TessBaseApiGetTsvText(Handle, pageNumber);
        try
        {
            return Marshal.PtrToStringUTF8(textPtr);
        }
        finally
        {
            TesseractNative.TessDeleteText(textPtr);
        }
    }

    public string? GetLstmText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var textPtr = TesseractNative.TessBaseApiGetLstmBoxText(Handle, pageNumber);
        try
        {
            return Marshal.PtrToStringUTF8(textPtr);
        }
        finally
        {
            TesseractNative.TessDeleteText(textPtr);
        }
    }

    public string? GetBoxText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var textPtr = TesseractNative.TessBaseApiGetBoxText(Handle, pageNumber);
        try
        {
            return Marshal.PtrToStringUTF8(textPtr);
        }
        finally
        {
            TesseractNative.TessDeleteText(textPtr);
        }
    }

    public void SetSegmentationMode(PageSegmentationMode mode)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetPageSegMode(Handle, mode);
    }

    public bool TryInitialization(string dataPath, string language)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(dataPath);
        ArgumentNullException.ThrowIfNull(language);
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
        ArgumentNullException.ThrowIfNull(dataPath);
        ArgumentNullException.ThrowIfNull(language);
        return TesseractNative.TessBaseApiInit2(Handle, dataPath, language, oem) == 0;
    }

    public void SetImage(IPix image)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(image);
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
        TesseractNative.TessBaseApiSetRectangle(Handle, left, top, width, height);
    }

    public unsafe void SetImage(byte[] imageData, int width, int height, int bytesPerPixel)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(imageData);

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
        return new TesseractPageIterator(iterator, false);
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