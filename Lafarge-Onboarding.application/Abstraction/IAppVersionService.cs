namespace Lafarge_Onboarding.application.Abstraction;

public interface IAppVersionService
{
    Task<AppVersionResponse> CreateAppVersionAsync(AppVersionRequest request);
    Task<AppVersionResponse?> GetLatestAppVersionAsync(string appName);
}