namespace Lafarge_Onboarding.application.Abstraction;

public interface IUsersRepository
{
    Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersAsync(PaginationRequest pagination);
    Task<int> UploadBulkUsersAsync(IEnumerable<Users> users);
    Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersByRoleAsync(string role, PaginationRequest pagination);
    Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersByNameAsync(string name, PaginationRequest pagination);
    Task<Users?> GetUserByIdAsync(string id);
}