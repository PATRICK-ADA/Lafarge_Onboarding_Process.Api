namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record WelcomeMessageResponse
{
    public WelcomePerson Ceo { get; init; } = new();
    public WelcomePerson Hr { get; init; } = new();
}

public sealed record WelcomePerson
{
    public string Name { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}