

using System.ComponentModel.DataAnnotations;

namespace Lafarge_Onboarding.domain.OnboardingRequests;

public sealed record AuthRegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public required string Password { get; init; }

    [Required(ErrorMessage = "FirstName is required")]
    public required string FirstName { get; init; }

    [Required(ErrorMessage = "LastName is required")]
    public required string LastName { get; init; }

    public string? PhoneNumber { get; init; }

    [Required(ErrorMessage = "Role is required")]
    public required string Role { get; init; } = UserRoles.LocalHire; // LOCAL_HIRE, EXPAT, VISITOR, HR_ADMIN

    public bool ActiveStatus { get; init; } = true;
}