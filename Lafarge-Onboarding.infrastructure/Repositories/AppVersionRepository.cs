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

    public async Task<AppVersion?> GetLatestAsync(string appName)
    {
        return await _context.AppVersions
            .Where(av => av.AppName == appName)
            .OrderByDescending(av => av.Version)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(AppVersion appVersion)
    {
        appVersion.UpdatedAt = DateTime.UtcNow;
        _context.AppVersions.Update(appVersion);
        await _context.SaveChangesAsync();
    }
}