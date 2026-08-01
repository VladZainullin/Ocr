namespace Vlad.Tesseract;

public enum PolyBlockType
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