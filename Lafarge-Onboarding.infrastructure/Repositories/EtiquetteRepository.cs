
namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class EtiquetteRepository : IEtiquetteRepository
{
    private readonly ApplicationDbContext _context;

    public EtiquetteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Etiquette etiquette)
    {
        await _context.Etiquettes.AddAsync(etiquette);
        await _context.SaveChangesAsync();
    }

    public async Task<EtiquetteResponse?> GetLatestAsync()
    {
        var entity = await _context.Etiquettes
            .OrderByDescending(e => e.CreatedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (entity == null) return null;

        return new EtiquetteResponse
        {
            Id = entity.Id,
            RegionalInfo = JsonSerializer.Deserialize<List<RegionalInfoItem>>(entity.RegionalInfo) ?? new List<RegionalInfoItem>(),
            FirstImpression = JsonSerializer.Deserialize<List<FirstImpressionItem>>(entity.FirstImpression) ?? new List<FirstImpressionItem>()
        };
    }

    public async Task UpdateAsync(Etiquette etiquette)
    {
        etiquette.UpdatedAt = DateTime.UtcNow;
        _context.Etiquettes.Update(etiquette);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLatestAsync()
    {
        var latest = await _context.Etiquettes.OrderByDescending(e => e.CreatedAt).FirstOrDefaultAsync();
        if (latest != null)
        {
            _context.Etiquettes.Remove(latest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        await _context.Etiquettes.ExecuteDeleteAsync();
    }
}