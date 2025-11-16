namespace Lafarge_Onboarding.application.Abstraction;

public interface IContactService
{
    Task UploadContactsAsync(IFormFile file);
    Task<List<ContactDto>> GetLocalContactsAsync();
    Task DeleteAllContactsAsync();
}