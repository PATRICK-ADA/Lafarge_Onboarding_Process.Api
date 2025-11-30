namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record AllContactsResponse
{
    public List<EmergencyContact> Emergency { get; init; } = new();
    public List<LafargeContact> Lafarge { get; init; } = new();
    public List<EmbassyContact> Embassies { get; init; } = new();
    public List<HrContact> Hr { get; init; } = new();
}

public sealed record EmergencyContact
{
    public string Service { get; init; } = string.Empty;
    public string Info { get; init; } = string.Empty;
}

public sealed record LafargeContact
{
    public string Function { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public sealed record EmbassyContact
{
    public string Embassy { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Website { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public sealed record HrContact
{
    public string Name { get; init; } = string.Empty;
    public string Designation { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}