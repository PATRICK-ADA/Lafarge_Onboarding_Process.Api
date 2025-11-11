namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public class WelcomeMessageResponse
{
    public WelcomePerson Ceo { get; set; } = new();
    public WelcomePerson Hr { get; set; } = new();
}

public class WelcomePerson
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}