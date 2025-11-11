namespace Lafarge_Onboarding.domain.Entities;

public class OnboardingPlan
{
    public int Id { get; set; }
    public string BuddyDetails { get; set; } = string.Empty;
    public string BuddyActivities { get; set; } = string.Empty; // JSON string for list
    public string ChecklistSummary { get; set; } = string.Empty;
    public string Timeline { get; set; } = string.Empty; // JSON string for list of timeline items
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}