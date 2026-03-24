namespace Domain;

public sealed class BlockModel
{
    public required string Text { get; set; } = null!;
    
    public required IReadOnlyCollection<LineModel> Lines { get; init; } = [];
}