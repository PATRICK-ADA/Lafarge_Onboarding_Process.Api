namespace Lafarge_Onboarding.domain.Entities;

public class AllContact
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty; // emergency, lafarge, embassies, hr
    public string Data { get; set; } = string.Empty; // JSON string
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}