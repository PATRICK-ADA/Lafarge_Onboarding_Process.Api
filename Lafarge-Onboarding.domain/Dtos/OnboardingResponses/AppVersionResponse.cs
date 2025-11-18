namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record AppVersionResponse
{
    public int Id { get; init; }
    public double Version { get; init; }
    public string Link { get; init; } = string.Empty;
    public string Features { get; init; } = string.Empty;
    public string AppName { get; init; } = string.Empty;
    public bool IsCritical { get; init; }
}