namespace Lafarge_Onboarding.application.Abstraction;

using Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public interface IWelcomeMessageService
{
    Task<WelcomeMessageResponse> ExtractAndSaveWelcomeMessagesAsync(IFormFileCollection files);
    Task<WelcomeMessageResponse?> GetWelcomeMessagesAsync();
    Task DeleteLatestAsync();
}