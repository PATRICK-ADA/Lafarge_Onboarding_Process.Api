namespace Lafarge_Onboarding.application.Abstraction;

using Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public interface IOnboardingPlanService
{
    Task<OnboardingPlanResponse> ExtractAndSaveOnboardingPlanAsync(IFormFile file);
    Task<OnboardingPlanResponse?> GetOnboardingPlanAsync();
}