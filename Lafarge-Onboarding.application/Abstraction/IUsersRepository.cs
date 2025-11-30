namespace Lafarge_Onboarding.application.Abstraction;

public interface IUsersRepository
{
    Task<(IEnumerable<GetUserResponse> Users, int TotalCount)> GetUsersAsync(PaginationRequest pagination);
    Task<int> UploadBulkUsersAsync(IEnumerable<Users> users);
    Task<(IEnumerable<GetUserResponse> Users, int TotalCount)> GetUsersByRoleAsync(string role, PaginationRequest pagination);
    Task<(IEnumerable<GetUserResponse> Users, int TotalCount)> GetUsersByNameAsync(string name, PaginationRequest pagination);
    Task<GetUserResponse?> GetUserByIdAsync(string id);
    Task<bool> UpdateUserAsync(string id, UpdateUserRequest request);
    Task<int> UpdateBulkUsersAsync(IEnumerable<UpdateUserItem> users);
    Task<bool> DeleteUserAsync(string id);
    Task<int> DeleteUsersByRoleAsync(string role);
}