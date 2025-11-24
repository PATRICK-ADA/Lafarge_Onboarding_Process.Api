namespace Lafarge_Onboarding.application.Services;

public sealed class CachedLocalHireInfoService : ILocalHireInfoService
{
    private readonly ILocalHireInfoService _baseService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedLocalHireInfoService> _logger;
    private const string CacheKey = "LocalHireInfo_Latest";

    public CachedLocalHireInfoService(
        ILocalHireInfoService baseService,
        IMemoryCache cache,
        ILogger<CachedLocalHireInfoService> logger)
    {
        _baseService = baseService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<LocalHireInfoResponse> ExtractAndSaveLocalHireInfoAsync(IFormFile file)
    {
        var result = await _baseService.ExtractAndSaveLocalHireInfoAsync(file);
        _cache.Remove(CacheKey);
        _logger.LogInformation("Cache cleared after new local hire info upload");
        return result;
    }

    public async Task<LocalHireInfoResponse?> GetLocalHireInfoAsync()
    {
        if (_cache.TryGetValue(CacheKey, out LocalHireInfoResponse? cached))
        {
            _logger.LogInformation("Returning cached local hire info");
            return cached;
        }

        var result = await _baseService.GetLocalHireInfoAsync();
        if (result != null)
        {
            _cache.Set(CacheKey, result, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.High,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365) // Cache indefinitely until manual removal
            });
            _logger.LogInformation("Local hire info cached until next database update");
        }

        return result;
    }

    public async Task DeleteLatestAsync()
    {
        await _baseService.DeleteLatestAsync();
        _cache.Remove(CacheKey);
        _logger.LogInformation("Cache cleared after local hire info deletion");
    }
}