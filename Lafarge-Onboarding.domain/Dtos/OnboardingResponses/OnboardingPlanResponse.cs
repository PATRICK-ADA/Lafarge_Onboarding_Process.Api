namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public class OnboardingPlanResponse
{
    public Buddy Buddy { get; set; } = new();
    public Checklist Checklist { get; set; } = new();
}

public class Buddy
{
    public string Details { get; set; } = string.Empty;
    public List<string> Activities { get; set; } = new();
}

public class Checklist
{
    public string Summary { get; set; } = string.Empty;
    public List<TimelineItem> Timeline { get; set; } = new();
}

public class TimelineItem
{
    public string Period { get; set; } = string.Empty;
    public List<string> Tasks { get; set; } = new();
    public List<string> SubTasks { get; set; } = new();
}