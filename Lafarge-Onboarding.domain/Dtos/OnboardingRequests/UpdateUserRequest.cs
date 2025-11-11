using System.ComponentModel.DataAnnotations;

namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record UpdateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; init; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; init; }

    public string? PhoneNumber { get; init; }

    public string? Role { get; init; }

    public string? Department { get; init; }

    public string? OnboardingStatus { get; init; }

    public bool? IsActive { get; init; }
}