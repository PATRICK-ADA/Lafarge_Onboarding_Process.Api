namespace Lafarge_Onboarding.application.Abstraction;

using Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public interface IAllContactService
{
    Task UploadAllContactsAsync(IFormFile file);
    Task<AllContactsResponse> GetAllContactsAsync();
    Task DeleteAllContactsAsync();
}