namespace Vlad.Tesseract.Contracts;

public interface ITesseractPageIterator : IDisposable
{
    nint Handle { get; }

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
    bool TryGetBaseline(PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);
    bool TryGetBoundingBox(PageIteratorLevel level, out int left, out int top, out int right, out int bottom);
    PolygonBlockType GetBlockType();
    ITesseractPageIterator Copy();
}