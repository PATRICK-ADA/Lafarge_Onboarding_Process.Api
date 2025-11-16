
namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; init; }
}