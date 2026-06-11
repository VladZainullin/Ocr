namespace Domain;

public sealed class ImageModel
{
    public required string Text { get; init; }
    public required IReadOnlyCollection<BlockModel> Blocks { get; init; }
}
