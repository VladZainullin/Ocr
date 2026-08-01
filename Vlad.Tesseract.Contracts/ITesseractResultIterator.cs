namespace Vlad.Tesseract.Contracts;

public interface ITesseractResultIterator : ITesseractPageIterator
{
    string GetText(PageIteratorLevel level);

    float GetConfidence(PageIteratorLevel level);

    bool WordIsFromDictionary();

    bool WordIsNumeric();

    bool SymbolIsSuperscript();

    bool SymbolIsSubscript();

    bool SymbolIsDropcast();
}