namespace Lafarge_Onboarding.domain.OnboardingRequests;

public sealed class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int Skip => (PageNumber - 1) * PageSize;
}