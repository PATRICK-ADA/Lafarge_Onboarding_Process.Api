namespace Lafarge_Onboarding.application.Abstraction;

public interface IUsersRepository
{
    Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersAsync(PaginationRequest pagination);
    Task<int> UploadBulkUsersAsync(IEnumerable<Users> users);
    Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersByRoleAsync(string role, PaginationRequest pagination);
    Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersByNameAsync(string name, PaginationRequest pagination);
    Task<Users?> GetUserByIdAsync(string id);
    Task<bool> UpdateUserAsync(string id, UpdateUserRequest request);
    Task<int> UpdateUsersByRoleAsync(string role, UpdateBulkUsersRequest request);
    Task<bool> DeleteUserAsync(string id);
    Task<int> DeleteUsersByRoleAsync(string role);
}