namespace Vlad.Tesseract.Contracts;

public interface ITesseractChoiceIterator
{
    bool NextElement();

    string GetText();

    float GetConfidence();
}