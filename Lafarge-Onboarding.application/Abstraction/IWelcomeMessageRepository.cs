namespace Lafarge_Onboarding.application.Abstraction;

public interface IWelcomeMessageRepository
{
    Task AddAsync(WelcomeMessage welcomeMessage);
    Task<WelcomeMessage?> GetLatestAsync();
    Task UpdateAsync(WelcomeMessage welcomeMessage);
    Task DeleteLatestAsync();
}