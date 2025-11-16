namespace Lafarge_Onboarding.application.Abstraction;

using Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public interface ILocalHireInfoService
{
    Task<LocalHireInfoResponse> ExtractAndSaveLocalHireInfoAsync(IFormFile file);
    Task<LocalHireInfoResponse?> GetLocalHireInfoAsync();
    Task DeleteLatestAsync();
}