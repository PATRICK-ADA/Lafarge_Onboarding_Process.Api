namespace Lafarge_Onboarding.application.Services;

public sealed class AppVersionService : IAppVersionService
{
    private readonly IAppVersionRepository _repository;
    private readonly ILogger<AppVersionService> _logger;
    private readonly IAuditService _auditService;

    public AppVersionService(IAppVersionRepository repository, ILogger<AppVersionService> logger, IAuditService auditService)
    {
        _repository = repository;
        _logger = logger;
        _auditService = auditService;
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

        // Log audit event for app version creation
        await _auditService.LogAuditEventAsync("CREATE", "AppVersion", entity.Id.ToString(), null, "Success", null, JsonSerializer.Serialize(entity), null);

        return MapToResponse(entity);
    }

    public async Task<AppVersionResponse?> GetLatestAppVersionAsync(string appName)
    {
        _logger.LogInformation("Retrieving latest app version for {AppName}", appName);

        var response = await _repository.GetLatestAsync(appName);
        if (response == null)
        {
            _logger.LogInformation("No app version found for {AppName}", appName);
            return null;
        }

        _logger.LogInformation("Latest app version retrieved successfully");

        // Log audit event for app version retrieval
        await _auditService.LogAuditEventAsync("READ", "AppVersion", response.Id.ToString(), $"Latest app version retrieved: {response.AppName} v{response.Version}");

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
