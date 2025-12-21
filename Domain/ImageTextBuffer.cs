namespace Domain;

public sealed class ImageTextBuffer
{
    public required int Number { get; init; }

    public required Memory<byte> Memory { get; init; }
}