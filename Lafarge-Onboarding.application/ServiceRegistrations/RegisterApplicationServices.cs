

namespace Lafarge_Onboarding.application.ServiceRegistrations;

public static class RegisterApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDocumentsUploadService, Lafarge_Onboarding.application.Services.DocumentsUploadService>();
        services.AddScoped<IAuthService, Lafarge_Onboarding.application.Services.AuthService>();
        services.AddScoped<IUsersService, Lafarge_Onboarding.application.Services.UsersService>();
        services.AddScoped<IEmailService, Lafarge_Onboarding.application.Services.EmailService>();
        services.AddScoped<ILocalHireInfoService, Lafarge_Onboarding.application.Services.LocalHireInfoService>();
        services.AddScoped<IWelcomeMessageService, Lafarge_Onboarding.application.Services.WelcomeMessageService>();
        services.AddScoped<IOnboardingPlanService, Lafarge_Onboarding.application.Services.OnboardingPlanService>();
        services.AddScoped<IEtiquetteService, Lafarge_Onboarding.application.Services.EtiquetteService>();
        // Repository registration is handled in infrastructure layer

        return services;
    }
}
        