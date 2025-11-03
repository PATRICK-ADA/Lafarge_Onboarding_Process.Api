using Lafarge_Onboarding.domain.Entities;
using Lafarge_Onboarding.domain.OnboardingRequests;

namespace Lafarge_Onboarding.application.Abstraction;

public interface IUsersRepository
{
    Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersAsync(PaginationRequest pagination);
    Task<int> UploadBulkUsersAsync(IEnumerable<Users> users);
}