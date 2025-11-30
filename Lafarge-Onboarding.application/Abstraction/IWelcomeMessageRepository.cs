namespace Lafarge_Onboarding.application.Abstraction;

public interface IWelcomeMessageRepository
{
    Task AddAsync(WelcomeMessage welcomeMessage);
    Task<WelcomeMessageResponse?> GetLatestAsync();
    Task UpdateAsync(WelcomeMessage welcomeMessage);
    Task DeleteLatestAsync();
    Task DeleteAllAsync();
}