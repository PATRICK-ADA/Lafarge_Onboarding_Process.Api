namespace Lafarge_Onboarding.application.Services;

public sealed class CachedWelcomeMessageService : IWelcomeMessageService
{
    private readonly IWelcomeMessageService _baseService;
    private readonly IMemoryCache _cache;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CachedWelcomeMessageService> _logger;
    private const string CacheKey = "WelcomeMessage_Latest";

    public CachedWelcomeMessageService(
        IWelcomeMessageService baseService,
        IMemoryCache cache,
        IAuditService auditService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CachedWelcomeMessageService> logger)
    {
        _baseService = baseService;
        _cache = cache;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    private string GetStatus()
    {
        return _httpContextAccessor.HttpContext.Response.StatusCode >= 200 && _httpContextAccessor.HttpContext.Response.StatusCode < 300 ? "Success" : "Failed";
    }


    public async Task<WelcomeMessageResponse> ExtractAndSaveWelcomeMessagesAsync(IFormFileCollection files)
    {
        var result = await _baseService.ExtractAndSaveWelcomeMessagesAsync(files);
        _cache.Remove(CacheKey);
        _logger.LogInformation("Cache cleared after new welcome message upload");
        var status = GetStatus();
        await _auditService.LogAuditEventAsync("CREATE", "CachedWelcomeMessage", _httpContextAccessor.HttpContext?.Request?.Path.ToString(), status: status, newValues: JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<WelcomeMessageResponse?> GetWelcomeMessagesAsync()
    {
        if (_cache.TryGetValue(CacheKey, out WelcomeMessageResponse? cached))
        {
            _logger.LogInformation("Returning cached welcome message");
            var auditStatus = GetStatus();
            await _auditService.LogAuditEventAsync("READ", "CachedWelcomeMessage", _httpContextAccessor.HttpContext?.Request?.Path.ToString(), status: auditStatus);
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
        var status = GetStatus();
        await _auditService.LogAuditEventAsync("READ", "CachedWelcomeMessage", _httpContextAccessor.HttpContext?.Request?.Path.ToString(), status: status);
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
        var status = GetStatus();
        await _auditService.LogAuditEventAsync("DELETE", "CachedWelcomeMessage", _httpContextAccessor.HttpContext?.Request?.Path.ToString(), status: status, oldValues: oldValues, newValues: null);
    }
}