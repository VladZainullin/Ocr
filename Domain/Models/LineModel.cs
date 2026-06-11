namespace Domain.Models;

public sealed class LineModel
{
    public required string Text { get; init; } = null!;
    public required IReadOnlyCollection<string> Words { get; init; } = [];
}