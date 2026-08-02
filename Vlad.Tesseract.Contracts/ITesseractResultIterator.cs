namespace Vlad.Tesseract.Contracts;

public interface ITesseractResultIterator : ITesseractPageIterator
{
    ITesseractPageIterator GetPageIterator();
    
    ITesseractPageIterator GetPageIteratorConst();

    ITesseractChoiceIterator GetChoiceIterator();
    
    string WordRecognitionLanguage();
    
    string GetText(PageIteratorLevel level);

    float GetConfidence(PageIteratorLevel level);

    bool WordIsFromDictionary();

    bool WordIsNumeric();

    bool SymbolIsSuperscript();

    bool SymbolIsSubscript();

    bool SymbolIsDropcast();
}