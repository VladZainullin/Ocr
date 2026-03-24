namespace Domain;

public sealed class ImageModel
{
    public required IReadOnlyCollection<BlockModel> Blocks { get; init; }
}
