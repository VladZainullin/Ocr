namespace Web;

public sealed record TesseractOptions
{
    public string Path { get; init; } = null!;

    public string Language { get; init; } = null!;
}