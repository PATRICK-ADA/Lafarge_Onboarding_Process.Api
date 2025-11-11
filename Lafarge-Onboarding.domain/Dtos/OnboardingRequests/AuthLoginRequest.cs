using System.ComponentModel.DataAnnotations;

namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record AuthLoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; init; }
}