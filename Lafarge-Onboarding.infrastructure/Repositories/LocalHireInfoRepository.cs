namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class LocalHireInfoRepository : ILocalHireInfoRepository
{
    private readonly ApplicationDbContext _context;

    public LocalHireInfoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LocalHireInfo localHireInfo)
    {
        await _context.LocalHireInfos.AddAsync(localHireInfo);
        await _context.SaveChangesAsync();
    }

    public async Task<LocalHireInfo?> GetLatestAsync()
    {
        return await _context.LocalHireInfos
            .OrderByDescending(l => l.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(LocalHireInfo localHireInfo)
    {
        localHireInfo.UpdatedAt = DateTime.UtcNow;
        _context.LocalHireInfos.Update(localHireInfo);
        await _context.SaveChangesAsync();
    }
}