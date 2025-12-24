namespace Domain;

public sealed class PageModel
{
    public required int Number { get; init; }

    public required BlockModel[] Blocks { get; set; }

    public List<ImageModel> Images { get; set; } = [];
}