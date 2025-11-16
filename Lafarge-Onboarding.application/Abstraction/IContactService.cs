namespace Lafarge_Onboarding.application.Abstraction;

using Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public interface IContactService
{
    Task UploadContactsAsync(IFormFile file);
    Task<List<ContactDto>> GetLocalContactsAsync();
    Task DeleteAllContactsAsync();
}