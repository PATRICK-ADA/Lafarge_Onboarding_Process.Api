namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record UserLogin
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
