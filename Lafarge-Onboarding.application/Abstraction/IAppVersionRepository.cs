namespace Lafarge_Onboarding.application.Abstraction;

public interface IAppVersionRepository
{
    Task AddAsync(AppVersion appVersion);
    Task<AppVersion?> GetLatestAsync(string appName);
    Task UpdateAsync(AppVersion appVersion);
}