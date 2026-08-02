namespace Vlad.Tesseract.Contracts;

public interface ITesseractResultIterator : ITesseractPageIterator
{
    ITesseractPageIterator GetPageIterator();
    
    ITesseractPageIterator GetPageIteratorConst();

    ITesseractChoiceIterator GetChoiceIterator();
    
    string WordRecognitionLanguage();
    
    string GetText(PageIteratorLevel level);

    bool WordIsFromDictionary();

    bool WordIsNumeric();

    bool SymbolIsSuperscript();

    bool SymbolIsSubscript();

    bool TryGetWordFontAttributes(
        out string fontName,
        out bool isBold,
        out bool isItalic,
        out bool isUnderlined,
        out bool isMonospace,
        out bool serif,
        out bool smallCaps,
        out int pointSize,
        out int fontId);
}