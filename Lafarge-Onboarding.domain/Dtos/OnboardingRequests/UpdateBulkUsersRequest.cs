using System.ComponentModel.DataAnnotations;

public sealed record UpdateBulkUsersRequest
{
    [Required(ErrorMessage = "Role is required")]
    public required string Role { get; init; }

    public string? Department { get; init; }

    public string? OnboardingStatus { get; init; }

    public bool? IsActive { get; init; }
}