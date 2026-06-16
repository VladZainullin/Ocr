namespace Domain.Models;

public sealed class ImageModel
{
    internal ImageModel()
    {
    }
    
    public required string Text { get; init; }
    public required IReadOnlyCollection<BlockModel> Blocks { get; init; }
}
