namespace Domain;

public sealed class PageModel
{
    public required int Number { get; init; }

    public required IReadOnlyCollection<BlockModel> Blocks { get; init; }

    public List<ImageModel> Images { get; init; } = [];
}