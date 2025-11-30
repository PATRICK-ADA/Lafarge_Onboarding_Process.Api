
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

    public async Task<WelcomeMessageResponse?> GetLatestAsync()
    {
        return await _context.WelcomeMessages
            .OrderByDescending(w => w.CreatedAt)
            .AsNoTracking()
            .Select(w => new WelcomeMessageResponse
            {
                Ceo = new WelcomePerson
                {
                    Name = w.CeoName,
                    Title = w.CeoTitle,
                    ImageUrl = w.CeoImageUrl,
                    Message = w.CeoMessage
                },
                Hr = new WelcomePerson
                {
                    Name = w.HrName,
                    Title = w.HrTitle,
                    ImageUrl = w.HrImageUrl,
                    Message = w.HrMessage
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(WelcomeMessage welcomeMessage)
    {
        welcomeMessage.UpdatedAt = DateTime.UtcNow;
        _context.WelcomeMessages.Update(welcomeMessage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLatestAsync()
    {
        var latest = await _context.WelcomeMessages.OrderByDescending(w => w.CreatedAt).FirstOrDefaultAsync();
        if (latest != null)
        {
            _context.WelcomeMessages.Remove(latest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        await _context.WelcomeMessages.ExecuteDeleteAsync();
    }
}