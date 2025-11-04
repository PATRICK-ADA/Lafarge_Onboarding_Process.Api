namespace Lafarge_Onboarding.domain.OnboardingResponses;

public class AuthLoginResponse
{
    public string Token { get; set; } = string.Empty;
    public AuthUserInfo User { get; set; } = new();
}

public class AuthUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}