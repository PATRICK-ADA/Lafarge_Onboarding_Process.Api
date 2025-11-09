namespace Lafarge_Onboarding.domain.OnboardingResponses;

public sealed record PaginatedResponse<T>
{
    public IEnumerable<T> Content { get; init; } = new List<T>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}