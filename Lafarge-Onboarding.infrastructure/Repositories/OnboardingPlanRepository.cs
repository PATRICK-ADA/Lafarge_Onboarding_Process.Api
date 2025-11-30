
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

    public async Task<OnboardingPlanResponse?> GetLatestAsync()
    {
        var entity = await _context.OnboardingPlans
            .OrderByDescending(o => o.CreatedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (entity == null) return null;

        return new OnboardingPlanResponse
        {
            Id = entity.Id,
            Buddy = new Buddy
            {
                Details = entity.BuddyDetails,
                Activities = JsonSerializer.Deserialize<List<string>>(entity.BuddyActivities) ?? new List<string>()
            },
            Checklist = new Checklist
            {
                Summary = entity.ChecklistSummary,
                Timeline = JsonSerializer.Deserialize<List<TimelineItem>>(entity.Timeline) ?? new List<TimelineItem>()
            }
        };
    }

    public async Task UpdateAsync(OnboardingPlan onboardingPlan)
    {
        onboardingPlan.UpdatedAt = DateTime.UtcNow;
        _context.OnboardingPlans.Update(onboardingPlan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLatestAsync()
    {
        var latest = await _context.OnboardingPlans.OrderByDescending(o => o.CreatedAt).FirstOrDefaultAsync();
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