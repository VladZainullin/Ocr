namespace Domain;

public sealed class BlockModel
{
    public required string Text { get; init; } = null!;
    
    public required IReadOnlyCollection<LineModel> Lines { get; init; }
}