namespace Lafarge_Onboarding.application.Abstraction;

public interface IEmailService
{
    Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken);
}