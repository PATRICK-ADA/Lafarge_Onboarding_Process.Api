namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class ContactRepository : IContactRepository
{
    private readonly ApplicationDbContext _context;

    public ContactRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Contact> contacts)
    {
        await _context.Contacts.AddRangeAsync(contacts);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Contact>> GetAllAsync()
    {
        return await _context.Contacts.ToListAsync();
    }

    public async Task DeleteAllAsync()
    {
        _context.Contacts.RemoveRange(_context.Contacts);
        await _context.SaveChangesAsync();
    }
}