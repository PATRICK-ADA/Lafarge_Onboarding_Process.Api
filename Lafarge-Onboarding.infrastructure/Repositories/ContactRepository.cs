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

    public async Task<List<ContactDto>> GetAllAsync()
    {
        return await _context.Contacts
            .AsNoTracking()
            .Select(c => new ContactDto
            {
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone
            })
            .ToListAsync();
    }

    public async Task DeleteAllAsync()
    {
        _context.Contacts.RemoveRange(_context.Contacts);
        await _context.SaveChangesAsync();
    }
}