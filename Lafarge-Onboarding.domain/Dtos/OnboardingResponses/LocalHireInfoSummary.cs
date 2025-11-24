namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed class LocalHireInfoSummary
{
    public int Id { get; set; }
    public string WhoWeAre { get; set; } = string.Empty;
    public string FootprintSummary { get; set; } = string.Empty;
    public string CultureSummary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}