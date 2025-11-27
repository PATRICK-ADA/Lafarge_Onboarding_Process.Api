namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public class LocalHireInfoResponse
{
    public AboutLafarge AboutLafarge { get; set; } = new();
    public GeneralIntro GeneralIntro { get; set; } = new();
}

public class AboutLafarge
{
    public string WhoWeAre { get; set; } = string.Empty;
    public Footprint Footprint { get; set; } = new();
    public Culture Culture { get; set; } = new();
}

public class Footprint
{
    public string Summary { get; set; } = string.Empty;
    public List<string> Plants { get; set; } = new();
    public List<string> ReadyMix { get; set; } = new();
    public string Depots { get; set; } = string.Empty;
}

public class Culture
{
    public string Summary { get; set; } = string.Empty;
    public string Pillars { get; set; } = string.Empty;
    public string Innovation { get; set; } = string.Empty;
    public List<string> HuaxinSpirit { get; set; } = new();
    public string RespectfulWorkplaces { get; set; } = string.Empty;
}

public class GeneralIntro
{
    public string Introduction { get; set; } = string.Empty;
    public List<CountryFact> CountryFacts { get; set; } = new();
    public List<string> InterestingFacts { get; set; } = new();
    public List<Holiday> Holidays { get; set; } = new();
}

public class CountryFact
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class Holiday
{
    public string Date { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}