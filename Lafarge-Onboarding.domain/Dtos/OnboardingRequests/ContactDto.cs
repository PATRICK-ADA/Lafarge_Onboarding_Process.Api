namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record ContactDto
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
}