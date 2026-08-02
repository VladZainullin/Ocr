namespace Vlad.Tesseract.Contracts;

public interface ITesseractResultIterator : ITesseractPageIterator
{
    ITesseractPageIterator GetPageIterator();
    
    ITesseractPageIterator GetPageIteratorConst();

    ITesseractChoiceIterator GetChoiceIterator();
    
    string? GetText(PageIteratorLevel level);
    
    string? WordRecognitionLanguage();
    
    string? GetWordFontAttributes(out bool isBold, out bool isItalic, out bool isUnderlined, out bool isMonospace,
        out bool isSerif, out bool isSmallCaps, out int pointSize, out int fontId);

    bool WordIsFromDictionary();

    bool IsWordNumeric();

    bool IsSymbolSuperscript();

    bool IsSymbolSubscript();

    bool IsSymbolDropCap();
}