
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

    public async Task<LocalHireInfoResponse?> GetLatestAsync()
    {
        var entity = await _context.LocalHireInfos
            .OrderByDescending(l => l.CreatedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (entity == null) return null;

        return new LocalHireInfoResponse
        {
            Id = entity.Id,
            AboutLafarge = new AboutLafarge
            {
                WhoWeAre = entity.WhoWeAre,
                Footprint = new Footprint
                {
                    Summary = entity.FootprintSummary,
                    Plants = JsonSerializer.Deserialize<List<string>>(entity.Plants) ?? new List<string>(),
                    ReadyMix = JsonSerializer.Deserialize<List<string>>(entity.ReadyMix) ?? new List<string>(),
                    Depots = entity.Depots
                },
                Culture = new Culture
                {
                    Summary = entity.CultureSummary,
                    Pillars = entity.Pillars,
                    Innovation = entity.Innovation,
                    HuaxinSpirit = JsonSerializer.Deserialize<List<string>>(entity.HuaxinSpirit) ?? new List<string>(),
                    RespectfulWorkplaces = entity.RespectfulWorkplaces
                }
            },
            GeneralIntro = new GeneralIntro
            {
                Introduction = entity.Introduction,
                CountryFacts = JsonSerializer.Deserialize<List<CountryFact>>(entity.CountryFacts) ?? new List<CountryFact>(),
                InterestingFacts = JsonSerializer.Deserialize<List<string>>(entity.InterestingFacts) ?? new List<string>(),
                Holidays = JsonSerializer.Deserialize<List<Holiday>>(entity.Holidays) ?? new List<Holiday>()
            }
        };
    }

    public async Task UpdateAsync(LocalHireInfo localHireInfo)
    {
        localHireInfo.UpdatedAt = DateTime.UtcNow;
        _context.LocalHireInfos.Update(localHireInfo);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLatestAsync()
    {
        var latest = await _context.LocalHireInfos.OrderByDescending(l => l.CreatedAt).FirstOrDefaultAsync();
        if (latest != null)
        {
            _context.LocalHireInfos.Remove(latest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        await _context.LocalHireInfos.ExecuteDeleteAsync();
    }
}