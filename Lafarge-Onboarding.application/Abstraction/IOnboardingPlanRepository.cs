namespace Lafarge_Onboarding.application.Abstraction;

public interface IOnboardingPlanRepository
{
    Task AddAsync(OnboardingPlan onboardingPlan);
    Task<OnboardingPlan?> GetLatestAsync();
    Task UpdateAsync(OnboardingPlan onboardingPlan);
    Task DeleteLatestAsync();
}