namespace Application.Contracts.Features.Images.Commands.RecognitionTextFromImage;

public sealed class RecognitionTextFromImageForm
{
    public required Stream Stream { get; init; }
}