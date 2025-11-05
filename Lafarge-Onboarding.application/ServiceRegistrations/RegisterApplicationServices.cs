

namespace Lafarge_Onboarding.application.ServiceRegistrations;

public static class RegisterApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDocumentsUploadService, Lafarge_Onboarding.application.Services.DocumentsUploadService>();
        services.AddScoped<IAuthService, Lafarge_Onboarding.application.Services.AuthService>();
        services.AddScoped<IUsersService, Lafarge_Onboarding.application.Services.UsersService>();

        return services;
    }
}
        