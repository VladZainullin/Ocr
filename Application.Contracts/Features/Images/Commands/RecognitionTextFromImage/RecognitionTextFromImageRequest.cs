using MediatR;

namespace Application.Contracts.Features.Images.Commands.RecognitionTextFromImage;

public sealed record RecognitionTextFromImageRequest(RecognitionTextFromImageForm Form) : IRequest<RecognitionTextFromImageResponse>;