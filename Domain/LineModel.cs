namespace Domain;

public sealed class LineModel
{
    public string Text { get; set; } = null!;
    public List<string> Words { get; } = [];
}