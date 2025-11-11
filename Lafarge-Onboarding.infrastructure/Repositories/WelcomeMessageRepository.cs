namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class WelcomeMessageRepository : IWelcomeMessageRepository
{
    private readonly ApplicationDbContext _context;

    public WelcomeMessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WelcomeMessage welcomeMessage)
    {
        await _context.WelcomeMessages.AddAsync(welcomeMessage);
        await _context.SaveChangesAsync();
    }

    public async Task<WelcomeMessage?> GetLatestAsync()
    {
        return await _context.WelcomeMessages
            .OrderByDescending(w => w.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(WelcomeMessage welcomeMessage)
    {
        welcomeMessage.UpdatedAt = DateTime.UtcNow;
        _context.WelcomeMessages.Update(welcomeMessage);
        await _context.SaveChangesAsync();
    }
}