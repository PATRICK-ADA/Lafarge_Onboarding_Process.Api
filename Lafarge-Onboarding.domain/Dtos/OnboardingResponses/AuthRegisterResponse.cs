namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record AuthRegisterResponse
{
    public string RegisterationStatus { get; init; } = "User registered successfully";
}