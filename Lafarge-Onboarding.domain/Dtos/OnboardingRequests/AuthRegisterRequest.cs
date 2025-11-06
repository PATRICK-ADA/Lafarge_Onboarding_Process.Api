

using System.ComponentModel.DataAnnotations;

namespace Lafarge_Onboarding.domain.OnboardingRequests;

public sealed class AuthRegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "FirstName is required")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is required")]
    public required string LastName { get; set; }

    public string? Department { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public required string Role { get; set; } = UserRoles.LocalHire; // LOCAL_HIRE, EXPAT, VISITOR, HR_ADMIN
}