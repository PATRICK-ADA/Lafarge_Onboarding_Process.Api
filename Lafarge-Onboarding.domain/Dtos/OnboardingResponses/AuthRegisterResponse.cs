namespace Lafarge_Onboarding.domain.OnboardingResponses;

public sealed record AuthRegisterResponse
{
    public string RegisterationStatus { get; init; } = "User registered successfully";
}