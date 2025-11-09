namespace Lafarge_Onboarding.domain.OnboardingResponses;

public sealed record AuthLoginResponse
{
    public string Token { get; init; } = string.Empty;
    public AuthUserInfo User { get; init; } = new();
}

public sealed record AuthUserInfo
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}