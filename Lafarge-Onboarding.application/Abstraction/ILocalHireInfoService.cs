namespace Lafarge_Onboarding.application.Abstraction;

public interface ILocalHireInfoService
{
    Task<LocalHireInfoResponse> ExtractAndSaveLocalHireInfoAsync(IFormFile file);
    Task<LocalHireInfoResponse?> GetLocalHireInfoAsync();
    Task DeleteLatestAsync();
}