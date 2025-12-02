namespace Lafarge_Onboarding.application.Services;

public sealed class CachedWelcomeMessageService : IWelcomeMessageService
{
    private readonly IWelcomeMessageService _baseService;
    private readonly IMemoryCache _cache;
    private readonly IAuditService _auditService;
    private readonly ILogger<CachedWelcomeMessageService> _logger;
    private const string CacheKey = "WelcomeMessage_Latest";

    public CachedWelcomeMessageService(
        IWelcomeMessageService baseService,
        IMemoryCache cache,
        IAuditService auditService,
        ILogger<CachedWelcomeMessageService> logger)
    {
        _baseService = baseService;
        _cache = cache;
        _auditService = auditService;
        _logger = logger;
    }


    public async Task<WelcomeMessageResponse> ExtractAndSaveWelcomeMessagesAsync(IFormFileCollection files)
    {
        var result = await _baseService.ExtractAndSaveWelcomeMessagesAsync(files);
        _cache.Remove(CacheKey);
        _logger.LogInformation("Cache cleared after new welcome message upload");
        await _auditService.LogAuditEventAsync("CREATE", "CachedWelcomeMessage", null, newValues: JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<WelcomeMessageResponse?> GetWelcomeMessagesAsync()
    {
        if (_cache.TryGetValue(CacheKey, out WelcomeMessageResponse? cached))
        {
            _logger.LogInformation("Returning cached welcome message");
            await _auditService.LogAuditEventAsync("READ", "CachedWelcomeMessage", null);
            return cached;
        }

        var result = await _baseService.GetWelcomeMessagesAsync();
        if (result != null)
        {
            _cache.Set(CacheKey, result, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.High,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365) // Cache indefinitely until manual removal
            });
            _logger.LogInformation("Welcome message cached until next database update");
        }
        await _auditService.LogAuditEventAsync("READ", "CachedWelcomeMessage", null);
        return result;
    }

    public async Task DeleteLatestAsync()
    {
        var entity = await _baseService.GetWelcomeMessagesAsync();
        string? oldValues = null;
        if (entity != null)
        {
            oldValues = JsonSerializer.Serialize(entity);
        }
        await _baseService.DeleteLatestAsync();
        _cache.Remove(CacheKey);
        _logger.LogInformation("Cache cleared after welcome message deletion");
        await _auditService.LogAuditEventAsync("DELETE", "CachedWelcomeMessage", null, oldValues: oldValues, newValues: null);
    }
}