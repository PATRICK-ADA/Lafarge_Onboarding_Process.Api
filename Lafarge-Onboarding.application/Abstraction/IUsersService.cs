namespace Lafarge_Onboarding.application.Abstraction;

public interface IUsersService
{
    Task<PaginatedResponse<GetUserResponse>> GetUsersAsync(PaginationRequest pagination);
    Task<ApiResponse<string>> UploadBulkUsersAsync(UploadBulkUsersRequests request);
    Task<PaginatedResponse<GetUserResponse>> GetUsersByRoleAsync(string role, PaginationRequest pagination);
    Task<PaginatedResponse<GetUserResponse>> GetUsersByNameAsync(string name, PaginationRequest pagination);
    Task<ApiResponse<GetUserResponse>> GetUserByIdAsync(string id);
    Task<ApiResponse<string>> UpdateUserByIdAsync(string id, UpdateUserRequest request);
    Task<ApiResponse<string>> UpdateBulkUsersByRoleAsync(UpdateBulkUsersRequest request);
    Task<ApiResponse<string>> DeleteUserByIdAsync(string id);
    Task<ApiResponse<string>> DeleteBulkUsersByRoleAsync(string role);
}