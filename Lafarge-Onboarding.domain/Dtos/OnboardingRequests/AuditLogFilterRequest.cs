namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record AuditLogFilterRequest
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string? UserEmail { get; init; }
    public string? UserRole { get; init; }
    public string? Action { get; init; }
    public string? ResourceType { get; init; }
    public string? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
