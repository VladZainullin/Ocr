namespace Web;

public sealed record TesseractOptions
{
    public required string Path { get; init; }

    public required string Language { get; init; }
}