
namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record UpdateBulkUsersRequest
{
    [Required(ErrorMessage = "Users is required")]
    public required List<UpdateUserItem> Users { get; init; }
}