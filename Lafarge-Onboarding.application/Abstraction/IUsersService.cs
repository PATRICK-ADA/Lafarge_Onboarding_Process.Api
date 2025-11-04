using Lafarge_Onboarding.domain.OnboardingRequests;
using Lafarge_Onboarding.domain.OnboardingResponses;

namespace Lafarge_Onboarding.application.Abstraction;

public interface IUsersService
{
    Task<PaginatedResponse<GetUserResponse>> GetUsersAsync(PaginationRequest pagination);
    Task<ApiResponse<string>> UploadBulkUsersAsync(UploadBulkUsersRequests request);
    Task<PaginatedResponse<GetUserResponse>> GetUsersByRoleAsync(string role, PaginationRequest pagination);
    Task<PaginatedResponse<GetUserResponse>> GetUsersByNameAsync(string name, PaginationRequest pagination);
    Task<ApiResponse<GetUserResponse>> GetUserByIdAsync(string id);
}