namespace Lafarge_Onboarding.domain.Entities;

public class LocalHireInfo
{
    public int Id { get; set; }
    public string WhoWeAre { get; set; } = string.Empty;
    public string FootprintSummary { get; set; } = string.Empty;
    public string Plants { get; set; } = string.Empty; 
    public string ReadyMix { get; set; } = string.Empty; 
    public string Depots { get; set; } = string.Empty;
    public string CultureSummary { get; set; } = string.Empty;
    public string Pillars { get; set; } = string.Empty;
    public string Innovation { get; set; } = string.Empty;
    public string HuaxinSpirit { get; set; } = string.Empty;
    public string RespectfulWorkplaces { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string CountryFacts { get; set; } = string.Empty; 
    public string InterestingFacts { get; set; } = string.Empty; 
    public string Holidays { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}