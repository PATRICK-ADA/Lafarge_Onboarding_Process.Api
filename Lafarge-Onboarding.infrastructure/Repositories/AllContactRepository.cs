namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class AllContactRepository : IAllContactRepository
{
    private readonly ApplicationDbContext _context;

    public AllContactRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<AllContact> contacts)
    {
        await _context.AllContacts.AddRangeAsync(contacts);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AllContact>> GetAllAsync()
    {
        return await _context.AllContacts.AsNoTracking().ToListAsync();
    }

    public async Task DeleteAllAsync()
    {
        var allContacts = await _context.AllContacts.ToListAsync();
        _context.AllContacts.RemoveRange(allContacts);
        await _context.SaveChangesAsync();
    }
}