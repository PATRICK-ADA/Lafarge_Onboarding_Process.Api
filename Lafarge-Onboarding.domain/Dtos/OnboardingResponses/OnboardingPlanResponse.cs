namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record OnboardingPlanResponse
{
    public int Id { get; init; }
    public Buddy Buddy { get; init; } = new();
    public Checklist Checklist { get; init; } = new();
}

public sealed record Buddy
{
    public string Details { get; init; } = string.Empty;
    public List<string> Activities { get; init; } = new();
}

public sealed record Checklist
{
    public string Summary { get; init; } = string.Empty;
    public List<TimelineItem> Timeline { get; init; } = new();
}

public sealed record TimelineItem
{
    public string Period { get; init; } = string.Empty;
    public List<string> Tasks { get; init; } = new();
    public List<string> SubTasks { get; init; } = new();
}