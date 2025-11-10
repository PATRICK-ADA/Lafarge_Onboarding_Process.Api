namespace Lafarge_Onboarding.application.Abstraction;

public interface IUsersService
{
    Task<PaginatedResponse<GetUserResponse>> GetUsersAsync(PaginationRequest pagination);
    Task<string> UploadBulkUsersAsync(IFormFile file);
    Task<PaginatedResponse<GetUserResponse>> GetUsersByRoleAsync(string role, PaginationRequest pagination);
    Task<PaginatedResponse<GetUserResponse>> GetUsersByNameAsync(string name, PaginationRequest pagination);
    Task<GetUserResponse> GetUserByIdAsync(string id);
    Task<string> UpdateUserByIdAsync(string id, UpdateUserRequest request);
    Task<string> UpdateBulkUsersAsync(UpdateBulkUsersRequest request);
    Task<string> DeleteUserByIdAsync(string id);
    Task<string> DeleteBulkUsersByRoleAsync(string role);
}