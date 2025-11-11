

namespace Lafarge_Onboarding.infrastructure.RegisterServices;

public static class RegisterInfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        
        services.AddScoped<IDocumentsUploadRepository, Lafarge_Onboarding.infrastructure.Repositories.DocumentsUploadRepository>();
        services.AddScoped<IUsersRepository, Lafarge_Onboarding.infrastructure.Repositories.UsersRepository>();
        services.AddScoped<ILocalHireInfoRepository, Lafarge_Onboarding.infrastructure.Repositories.LocalHireInfoRepository>();

        return services;
    }
}