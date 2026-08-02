namespace Vlad.Tesseract.Contracts;

public interface ITesseractPageIterator : IDisposable
{
    nint Handle { get; set; }

    IPix GetBinaryImage(PageIteratorLevel level);

    IPix GetImage(PageIteratorLevel level, int padding, IPix originalImage, out int left, out int top);

    void GetParagraphInfo(
        out ParagraphJustification justification, out bool isListItem, out bool isCrown, out int firstLineIndent);

    void GetOrientation(out OrientationPage orientation, out WritingDirection writingDirection,
        out TextLineOrder textLineOrder, out float deskewAngle);
    void Begin();
    bool TryNext(PageIteratorLevel level);
    bool IsAtBeginningOf(PageIteratorLevel level);
    bool IsAtFinalElement(PageIteratorLevel level, PageIteratorLevel element);
    bool TryGetBaseLine(PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);
    bool TryGetBoundingBox(PageIteratorLevel level, out int x, out int y, out int width, out int height);
    PolygonBlockType GetBlockType();
    ITesseractPageIterator Copy();
}