namespace Domain;

public sealed class LineModel
{
    public required string Text { get; set; } = null!;
    public required IReadOnlyCollection<string> Words { get; init; }
}