namespace Vlad.Tesseract.Contracts;

public interface ITesseractEngine
{
    PageSegmentationMode PageSegmentationMode { get; }
    string InputName { get; }
    
    nint Handle { get; }
    string Text { get; }
    float MeanTextConfidence { get; }
    void SetVariable(string name, string value);
    void SetDebugVariable(string name, string value);
    void SetInputName(IPix pix);
    void SetInputName(string name);
    string? GetVariable(string name);
    bool TryGetVariable(string name, out int? value);
    bool TryGetVariable(string name, out double? value);
    bool TryGetVariable(string name, out bool? value);
    string GetHOcrText(int pageNumber);
    string GetAltoText(int pageNumber);
    string GetTsvText(int pageNumber);
    string GetLstmText(int pageNumber);
    string GetBoxText(int pageNumber);
    string GetUniChar(int uniCharId);
    void SetSegmentationMode(PageSegmentationMode mode);
    bool TryInitialization(string dataPath, string language);
    bool TryInitialization(string dataPath, string language, OcrEngineMode oem);
    int GetSourceYResolution();
    void SetSourceResolution(int ppi);
    void SetImage(IPix image);
    void SetImage(byte[] imageData, uint width, uint height, uint bytesPerPixel);
    void Recognize(ITesseractMonitor monitor);
    void SetRectangle(int left, int top, int width, int height);
    string GetInitializationLanguages();
    ITesseractResultIterator GetIterator();
    ITesseractPageIterator AnalyzeLayout();
    bool TryGetTextDirection(out int outOffset, out float slope);
    void SetMinimumOrientationMargin(double margin);
    void EndElement();
    void Clear();
    void ClearAdaptiveClassifier();
    bool IsValidWord(string word);
    IPix GetThresholdedImage();
}