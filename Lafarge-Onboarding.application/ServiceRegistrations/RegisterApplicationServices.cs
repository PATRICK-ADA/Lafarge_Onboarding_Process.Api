

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
        // Repository registration is handled in infrastructure layer

        return services;
    }
}
        