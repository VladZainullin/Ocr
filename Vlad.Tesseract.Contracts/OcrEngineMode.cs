namespace Vlad.Tesseract.Contracts;

public enum OcrEngineMode
{
    OemTesseractOnly = 0,
    OemLstmOnly = 1,
    OemTesseractLstmCombined = 2,
    OemDefault = 3
}