namespace Application.Contracts.Features.Images.Commands.RecognitionTextFromImage;

public sealed class RecognitionTextFromImageResponse
{
    public required List<Block> Blocks { get; set; }
}

public sealed class Block
{
    public List<Paragraph> Paragraphs { get; } = [];
    
    public sealed class Paragraph
    {
        public List<Line> Lines { get; } = [];
    }

    public sealed class Line
    {
        public List<string> Words { get; } = [];
    }
}