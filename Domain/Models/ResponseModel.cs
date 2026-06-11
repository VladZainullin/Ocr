namespace Domain.Models;

public sealed class ResponseModel
{
    public required IReadOnlyCollection<PageModel> Pages { get; init; }
}