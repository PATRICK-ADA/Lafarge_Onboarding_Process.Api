namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record LocalHireInfoResponse
{
    public int Id { get; init; }
    public AboutLafarge AboutLafarge { get; init; } = new();
    public GeneralIntro GeneralIntro { get; init; } = new();
}

public sealed record AboutLafarge
{
    public string WhoWeAre { get; init; } = string.Empty;
    public Footprint Footprint { get; init; } = new();
    public Culture Culture { get; init; } = new();
}

public sealed record Footprint
{
    public string Summary { get; init; } = string.Empty;
    public List<string> Plants { get; init; } = new();
    public List<string> ReadyMix { get; init; } = new();
    public string Depots { get; init; } = string.Empty;
}

public sealed record Culture
{
    public string Summary { get; init; } = string.Empty;
    public string Pillars { get; init; } = string.Empty;
    public string Innovation { get; init; } = string.Empty;
    public List<string> HuaxinSpirit { get; init; } = new();
    public string RespectfulWorkplaces { get; init; } = string.Empty;
}

public sealed record GeneralIntro
{
    public string Introduction { get; init; } = string.Empty;
    public List<CountryFact> CountryFacts { get; init; } = new();
    public List<string> InterestingFacts { get; init; } = new();
    public List<Holiday> Holidays { get; init; } = new();
}

public sealed record CountryFact
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

public sealed record Holiday
{
    public string Date { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}