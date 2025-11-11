namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public class EtiquetteResponse
{
    public List<RegionalInfoItem> RegionalInfo { get; set; } = new();
    public List<FirstImpressionItem> FirstImpression { get; set; } = new();
}

public class RegionalInfoItem
{
    public string Title { get; set; } = string.Empty;
    public List<Region> Regions { get; set; } = new();
}

public class Region
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class FirstImpressionItem
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}