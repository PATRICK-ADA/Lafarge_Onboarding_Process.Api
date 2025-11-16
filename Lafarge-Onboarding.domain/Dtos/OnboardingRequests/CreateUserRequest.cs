namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record CreateUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "FirstName is required")]
    public required string FirstName { get; init; }

    [Required(ErrorMessage = "LastName is required")]
    public required string LastName { get; init; }

    public string? PhoneNumber { get; init; }

    public string? StaffProfilePicture { get; init; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    public required string Role { get; init; } = UserRoles.LocalHire;

    public bool ActiveStatus { get; init; } = true;
}