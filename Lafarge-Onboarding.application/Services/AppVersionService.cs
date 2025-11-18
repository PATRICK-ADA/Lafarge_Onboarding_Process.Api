namespace Lafarge_Onboarding.application.Services;

public sealed class AppVersionService : IAppVersionService
{
    private readonly IAppVersionRepository _repository;
    private readonly ILogger<AppVersionService> _logger;

    public AppVersionService(IAppVersionRepository repository, ILogger<AppVersionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AppVersionResponse> CreateAppVersionAsync(AppVersionRequest request)
    {
        _logger.LogInformation("Creating new app version for {AppName}", request.AppName);

        var entity = new AppVersion
        {
            Version = request.Version,
            Link = request.Link,
            Features = request.Features,
            IsCritical = request.IsCritical,
            AppName = request.AppName
        };

        await _repository.AddAsync(entity);

        _logger.LogInformation("App version created successfully with ID: {Id}", entity.Id);

        return MapToResponse(entity);
    }

    public async Task<AppVersionResponse?> GetLatestAppVersionAsync(string appName)
    {
        _logger.LogInformation("Retrieving latest app version for {AppName}", appName);

        var entity = await _repository.GetLatestAsync(appName);
        if (entity == null)
        {
            _logger.LogInformation("No app version found for {AppName}", appName);
            return null;
        }

        var response = MapToResponse(entity);
        _logger.LogInformation("Latest app version retrieved successfully");
        return response;
    }

    private AppVersionResponse MapToResponse(AppVersion entity)
    {
        return new AppVersionResponse
        {
            Id = entity.Id,
            Version = entity.Version,
            Link = entity.Link,
            Features = entity.Features,
            AppName = entity.AppName,
            IsCritical = entity.IsCritical
        };
    }
}