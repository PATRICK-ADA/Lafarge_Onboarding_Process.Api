using System.ComponentModel.DataAnnotations;

namespace Lafarge_Onboarding.domain.OnboardingRequests;

public sealed class AuthLoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }
}