

namespace Lafarge_Onboarding.infrastructure.RegisterServices;

public static class RegisterInfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        
        services.AddScoped<IDocumentsUploadRepository, Lafarge_Onboarding.infrastructure.Repositories.DocumentsUploadRepository>();
        services.AddScoped<IUsersRepository, Lafarge_Onboarding.infrastructure.Repositories.UsersRepository>();
        services.AddScoped<ILocalHireInfoRepository, Lafarge_Onboarding.infrastructure.Repositories.LocalHireInfoRepository>();
        services.AddScoped<IWelcomeMessageRepository, Lafarge_Onboarding.infrastructure.Repositories.WelcomeMessageRepository>();
        services.AddScoped<IOnboardingPlanRepository, Lafarge_Onboarding.infrastructure.Repositories.OnboardingPlanRepository>();
        services.AddScoped<IEtiquetteRepository, Lafarge_Onboarding.infrastructure.Repositories.EtiquetteRepository>();
        services.AddScoped<IContactRepository, Lafarge_Onboarding.infrastructure.Repositories.ContactRepository>();
        services.AddScoped<IAllContactRepository, Lafarge_Onboarding.infrastructure.Repositories.AllContactRepository>();
        services.AddScoped<IGalleryRepository, Lafarge_Onboarding.infrastructure.Repositories.GalleryRepository>();
        services.AddScoped<IAppVersionRepository, Lafarge_Onboarding.infrastructure.Repositories.AppVersionRepository>();
        services.AddScoped<IAuditLogRepository, Lafarge_Onboarding.infrastructure.Repositories.AuditLogRepository>();

        return services;
    }
}