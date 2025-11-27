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

    public async Task<Etiquette?> GetLatestAsync()
    {
        return await _context.Etiquettes
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Etiquette etiquette)
    {
        etiquette.UpdatedAt = DateTime.UtcNow;
        _context.Etiquettes.Update(etiquette);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLatestAsync()
    {
        var latest = await GetLatestAsync();
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