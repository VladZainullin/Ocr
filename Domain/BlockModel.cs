namespace Domain;

public sealed class BlockModel
{
    public string Text { get; set; } = null!;
    
    public List<LineModel> Lines { get; } = [];
}