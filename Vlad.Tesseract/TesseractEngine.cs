using System.Runtime.InteropServices;
using Vlad.Tesseract.Contracts;

namespace Vlad.Tesseract;

internal sealed class TesseractEngine : IDisposable, ITesseractEngine
{
    private readonly IntPtr _handle = TesseractNative.TessBaseApiCreate();
    private bool _disposed;

    public static string Version => TesseractNative.TessVersion();

    public static string DataPath => TesseractNative.TessBaseApiGetDataPath();

    public PageSegmentationMode PageSegmentationMode
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiGetPageSegMode(_handle);
        }
    }

    public void SetVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetVariable(_handle, name, value);
    }

    public void SetDebugVariable(string name, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetDebugVariable(_handle, name, value);
    }

    public void SetInputName(IPix pix)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetInputName(_handle, pix.Handle);
    }

    public string? GetVariable(string name)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var pointer = TesseractNative.TessBaseApiGetStringVariable(
            _handle,
            name);

        return Marshal.PtrToStringUTF8(pointer);
    }
    
    public bool TryGetVariable(string name, out int? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (TesseractNative.TessBaseApiGetIntVariable(_handle, name, out var v))
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
        if (TesseractNative.TessBaseApiGetDoubleVariable(_handle, name, out var v))
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
        if (TesseractNative.TessBaseApiGetBoolVariable(_handle, name, out var v))
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
        TesseractNative.TessBaseApiSetInputName(_handle, name);
    }

    public string InputName
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiGetInputName(_handle);
        }
    }

    public string Text
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiGetUtf8Text(_handle);
        }
    }

    public float MeanTextConfidence
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return TesseractNative.TessBaseApiMeanTextConf(_handle);
        }
    }

    public string GetHOcrText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetHOcrText(_handle, pageNumber);
    }

    public string GetAltoText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetAltoText(_handle, pageNumber);
    }

    public string GetTsvText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetTsvText(_handle, pageNumber);
    }

    public string GetLstmText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetLstmBoxText(_handle, pageNumber);
    }

    public string GetBoxText(int pageNumber)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetBoxText(_handle, pageNumber);
    }

    public void SetSegmentationMode(PageSegmentationMode mode)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetPageSegMode(_handle, mode);
    }

    public bool TryInitialization(string dataPath, string language)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiInit3(_handle, dataPath, language);
    }

    public int GetSourceYResolution()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetSourceYResolution(_handle);
    }

    public void SetSourceResolution(int ppi)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetSourceResolution(_handle, ppi);
    }

    public bool TryInitialization(string dataPath, string language, TessOcrEngineMode oem)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiInit2(_handle, dataPath, language, oem);
    }

    public void SetImage(IPix image)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetImage2(_handle, image.Handle);
    }

    public void Recognize(ITesseractMonitor monitor)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(monitor);
        TesseractNative.TessBaseApiRecognize(_handle, monitor.Handle);
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
            TesseractNative.TessBaseApiSetImage(_handle, (nint)imagePtr, width, height, bytesPerPixel, bytesPerLine);
        }
    }

    public string GetInitializationLanguages()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetInitLanguagesAsString(_handle);
    }

    public ITesseractResultIterator GetIterator()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var iterator = TesseractNative.TessBaseApiGetIterator(_handle);
        return new TesseractResultIterator(iterator);
    }

    public ITesseractPageIterator AnalyzeLayout()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var iterator = TesseractNative.TessBaseApiAnalyseLayout(_handle);
        return new TesseractPageIterator(iterator);
    }

    public bool TryGetTextDirection(out int outOffset, out float slope)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetTextDirection(_handle, out outOffset, out slope);
    }

    public void SetMinimumOrientationMargin(double margin)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiSetMinOrientationMargin(_handle, margin);
    }

    public string GetUniChar(int uniCharId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiGetUnichar(_handle, uniCharId);
    }

    public void EndElement()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiEnd(_handle);
    }

    public void Clear()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiClear(_handle);
    }

    public void ClearAdaptiveClassifier()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        TesseractNative.TessBaseApiAdaptiveClassifier(_handle);
    }

    public bool IsValidWord(string word)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return TesseractNative.TessBaseApiIsValidWord(_handle, word);
    }

    public IPix GetThresholdedImage()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var pixPtr = TesseractNative.TessBaseApiGetThresholdedImage(_handle);
        return new Pix(pixPtr);
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_handle != IntPtr.Zero)
        {
            TesseractNative.TessBaseApiDelete(_handle);    
        }
        
        _disposed = true;
    }
}