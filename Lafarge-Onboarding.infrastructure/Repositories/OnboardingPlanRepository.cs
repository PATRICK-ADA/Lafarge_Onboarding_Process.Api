namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class OnboardingPlanRepository : IOnboardingPlanRepository
{
    private readonly ApplicationDbContext _context;

    public OnboardingPlanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OnboardingPlan onboardingPlan)
    {
        await _context.OnboardingPlans.AddAsync(onboardingPlan);
        await _context.SaveChangesAsync();
    }

    public async Task<OnboardingPlan?> GetLatestAsync()
    {
        return await _context.OnboardingPlans
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(OnboardingPlan onboardingPlan)
    {
        onboardingPlan.UpdatedAt = DateTime.UtcNow;
        _context.OnboardingPlans.Update(onboardingPlan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLatestAsync()
    {
        var latest = await GetLatestAsync();
        if (latest != null)
        {
            _context.OnboardingPlans.Remove(latest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        await _context.OnboardingPlans.ExecuteDeleteAsync();
    }
}