namespace Web.Models;

public sealed class ImageModel
{
    public required IEnumerable<BlockModel> Blocks { get; set; }
}