namespace Lafarge_Onboarding.application.Abstraction;
public interface IAllContactService
{
    Task UploadAllContactsAsync(IFormFile file);
    Task<AllContactsResponse> GetAllContactsAsync();
    Task DeleteAllContactsAsync();
}