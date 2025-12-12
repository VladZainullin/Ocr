namespace Web.Models;

public sealed class Page
{
    public required int Number { get; init; }

    public required List<Block> Blocks { get; set; }

    public List<Image> Images { get; } = [];
}