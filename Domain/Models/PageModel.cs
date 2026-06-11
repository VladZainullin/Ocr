namespace Domain.Models;

public sealed class PageModel
{
    public required int Number { get; init; }

    public required string Text { get; init; }

    public required IReadOnlyCollection<BlockModel> Blocks { get; init; }

    public required IReadOnlyCollection<ImageModel> Images { get; init; }
}