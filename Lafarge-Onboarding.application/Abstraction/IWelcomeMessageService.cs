namespace Lafarge_Onboarding.application.Abstraction;
public interface IWelcomeMessageService
{
    Task<WelcomeMessageResponse> ExtractAndSaveWelcomeMessagesAsync(IFormFileCollection files);
    Task<WelcomeMessageResponse?> GetWelcomeMessagesAsync();
    Task DeleteLatestAsync();
}