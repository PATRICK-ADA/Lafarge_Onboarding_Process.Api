

namespace Lafarge_Onboarding.application.ServiceRegistrations;

public static class RegisterApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDocumentsUploadService, Lafarge_Onboarding.application.Services.DocumentsUploadService>();
        services.AddScoped<IImprovedDocumentExtractionService, Lafarge_Onboarding.application.Services.ImprovedDocumentExtractionService>();
        services.AddScoped<IAuthService, Lafarge_Onboarding.application.Services.AuthService>();
        services.AddScoped<IUsersService, Lafarge_Onboarding.application.Services.UsersService>();
        services.AddScoped<IEmailService, Lafarge_Onboarding.application.Services.EmailService>();
        // Register base services
        services.AddScoped<Lafarge_Onboarding.application.Services.LocalHireInfoService>();
        services.AddScoped<Lafarge_Onboarding.application.Services.WelcomeMessageService>();
        
        // Register cached decorators
        services.AddScoped<ILocalHireInfoService>(provider =>
            new Lafarge_Onboarding.application.Services.CachedLocalHireInfoService(
                provider.GetRequiredService<Lafarge_Onboarding.application.Services.LocalHireInfoService>(),
                provider.GetRequiredService<IMemoryCache>(),
                provider.GetRequiredService<ILogger<Lafarge_Onboarding.application.Services.CachedLocalHireInfoService>>()));
                
        services.AddScoped<IWelcomeMessageService>(provider =>
            new Lafarge_Onboarding.application.Services.CachedWelcomeMessageService(
                provider.GetRequiredService<Lafarge_Onboarding.application.Services.WelcomeMessageService>(),
                provider.GetRequiredService<IMemoryCache>(),
                provider.GetRequiredService<ILogger<Lafarge_Onboarding.application.Services.CachedWelcomeMessageService>>()));
                
        services.AddScoped<IOnboardingPlanService, Lafarge_Onboarding.application.Services.OnboardingPlanService>();
        services.AddScoped<IEtiquetteService, Lafarge_Onboarding.application.Services.EtiquetteService>();
        services.AddScoped<IContactService, Lafarge_Onboarding.application.Services.ContactService>();
        services.AddScoped<IAllContactService, Lafarge_Onboarding.application.Services.AllContactService>();
        services.AddScoped<IGalleryService, Lafarge_Onboarding.application.Services.GalleryService>();
        services.AddScoped<IAppVersionService, Lafarge_Onboarding.application.Services.AppVersionService>();


        return services;
    }
}
        