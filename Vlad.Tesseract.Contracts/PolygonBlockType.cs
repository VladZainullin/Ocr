namespace Vlad.Tesseract.Contracts;

public enum PolygonBlockType
{
    Unknown = 0,
    FlowingText,
    HeadingText,
    PulloutText,
    Equation,
    InlineEquation,
    Table,
    VerticalText,
    CaptionText,
    FlowingImage,
    HeadingImage,
    PulloutImage,
    HorizontalLine,
    VerticalLine,
    Noise,
    Count
}