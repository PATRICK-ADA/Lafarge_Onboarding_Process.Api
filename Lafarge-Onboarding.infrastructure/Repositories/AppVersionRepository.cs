namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class AppVersionRepository : IAppVersionRepository
{
    private readonly ApplicationDbContext _context;

    public AppVersionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AppVersion appVersion)
    {
        await _context.AppVersions.AddAsync(appVersion);
        await _context.SaveChangesAsync();
    }

    public async Task<AppVersionResponse?> GetLatestAsync(string appName)
    {
        return await _context.AppVersions
            .Where(av => av.AppName == appName)
            .OrderByDescending(av => av.Version)
            .AsNoTracking()
            .Select(av => new AppVersionResponse
            {
                Id = av.Id,
                Version = av.Version,
                Link = av.Link,
                Features = av.Features,
                AppName = av.AppName,
                IsCritical = av.IsCritical
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(AppVersion appVersion)
    {
        appVersion.UpdatedAt = DateTime.UtcNow;
        _context.AppVersions.Update(appVersion);
        await _context.SaveChangesAsync();
    }
}