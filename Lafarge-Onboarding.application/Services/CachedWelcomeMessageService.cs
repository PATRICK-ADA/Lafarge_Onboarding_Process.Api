namespace Lafarge_Onboarding.application.Services;

public sealed class CachedWelcomeMessageService : IWelcomeMessageService
{
    private readonly IWelcomeMessageService _baseService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedWelcomeMessageService> _logger;
    private const string CacheKey = "WelcomeMessage_Latest";

    public CachedWelcomeMessageService(
        IWelcomeMessageService baseService,
        IMemoryCache cache,
        ILogger<CachedWelcomeMessageService> logger)
    {
        _baseService = baseService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<WelcomeMessageResponse> ExtractAndSaveWelcomeMessagesAsync(IFormFileCollection files)
    {
        var result = await _baseService.ExtractAndSaveWelcomeMessagesAsync(files);
        _cache.Remove(CacheKey);
        _logger.LogInformation("Cache cleared after new welcome message upload");
        return result;
    }

    public async Task<WelcomeMessageResponse?> GetWelcomeMessagesAsync()
    {
        if (_cache.TryGetValue(CacheKey, out WelcomeMessageResponse? cached))
        {
            _logger.LogInformation("Returning cached welcome message");
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

        return result;
    }

    public async Task DeleteLatestAsync()
    {
        await _baseService.DeleteLatestAsync();
        _cache.Remove(CacheKey);
        _logger.LogInformation("Cache cleared after welcome message deletion");
    }
}