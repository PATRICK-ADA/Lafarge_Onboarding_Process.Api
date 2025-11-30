namespace Lafarge_Onboarding.application.Abstraction;

public interface IAppVersionRepository
{
    Task AddAsync(AppVersion appVersion);
    Task<AppVersionResponse?> GetLatestAsync(string appName);
    Task UpdateAsync(AppVersion appVersion);
}