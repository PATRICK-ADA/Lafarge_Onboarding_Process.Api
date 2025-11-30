namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record EtiquetteResponse
{
    public int Id { get; init; }
    public List<RegionalInfoItem> RegionalInfo { get; init; } = new();
    public List<FirstImpressionItem> FirstImpression { get; init; } = new();
}

public sealed record RegionalInfoItem
{
    public string Title { get; init; } = string.Empty;
    public List<Region> Regions { get; init; } = new();
}

public sealed record Region
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}

public sealed record FirstImpressionItem
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}