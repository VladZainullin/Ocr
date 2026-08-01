namespace Vlad.Tesseract.Contracts;

public interface ITesseractPageIterator : IDisposable
{
    nint Iterator { get; set; }
    void Begin();
    bool NextElement(PageIteratorLevel level);
    bool IsAtBeginningOf(PageIteratorLevel level);
    bool IsAtFinalElement(PageIteratorLevel level, PageIteratorLevel element);
    bool TryGetBaseLine(PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);
    bool TryGetBoundingBox(PageIteratorLevel level, out int x, out int y, out int width, out int height);
    PolyBlockType GetBlockType();
    ITesseractPageIterator Copy();
}