namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record AppVersionRequest
{
    [Required(ErrorMessage = "Version is required")]
    public required double Version { get; init; }

    [Required(ErrorMessage = "Link is required")]
    public required string Link { get; init; }

    [Required(ErrorMessage = "Features is required")]
    public required string Features { get; init; }

    [Required(ErrorMessage = "IsCritical is required")]
    public required bool IsCritical { get; init; }

    [Required(ErrorMessage = "AppName is required")]
    public required string AppName { get; init; }
}