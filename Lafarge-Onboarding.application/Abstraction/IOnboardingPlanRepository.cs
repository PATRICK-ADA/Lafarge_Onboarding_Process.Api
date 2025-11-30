namespace Lafarge_Onboarding.application.Abstraction;

public interface IOnboardingPlanRepository
{
    Task AddAsync(OnboardingPlan onboardingPlan);
    Task<OnboardingPlanResponse?> GetLatestAsync();
    Task UpdateAsync(OnboardingPlan onboardingPlan);
    Task DeleteLatestAsync();
    Task DeleteAllAsync();
}