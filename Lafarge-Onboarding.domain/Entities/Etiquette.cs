namespace Lafarge_Onboarding.domain.Entities;

public class Etiquette
{
    public int Id { get; set; }
    public string RegionalInfo { get; set; } = string.Empty; // JSON string for list of regional info items
    public string FirstImpression { get; set; } = string.Empty; // JSON string for list of first impression items
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}