namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record PaginationRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public int Skip => (PageNumber - 1) * PageSize;
}