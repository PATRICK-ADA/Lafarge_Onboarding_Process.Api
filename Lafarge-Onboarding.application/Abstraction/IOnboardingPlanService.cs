namespace Lafarge_Onboarding.application.Abstraction;
public interface IOnboardingPlanService
{
    Task<OnboardingPlanResponse> ExtractAndSaveOnboardingPlanAsync(IFormFile file);
    Task<OnboardingPlanResponse?> GetOnboardingPlanAsync();
    Task DeleteLatestAsync();
}