
namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record UploadBulkUsersRequests
{
    [Required(ErrorMessage = "Users list is required")]
    [MinLength(1, ErrorMessage = "At least one user must be provided")]
    public List<CreateUserRequest> Users { get; init; } = new();
}