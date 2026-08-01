namespace Vlad.Tesseract.Contracts;

public interface IChoiceIterator
{
    bool NextElement();

    string GetText();

    float GetConfidence();
}