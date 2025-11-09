using System.ComponentModel.DataAnnotations;

namespace Lafarge_Onboarding.domain.OnboardingRequests;

public sealed record ResetPasswordRequest
{
    [Required(ErrorMessage = "Token is required")]
    public required string Token { get; init; }

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public required string NewPassword { get; init; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; init; }
}