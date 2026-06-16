namespace Domain.Models;

public sealed class BlockModel
{
    internal BlockModel()
    {
    }
    
    public required string Text { get; init; } = null!;
    
    public required IReadOnlyCollection<LineModel> Lines { get; init; }
}