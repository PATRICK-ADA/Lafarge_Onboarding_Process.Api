namespace Lafarge_Onboarding.domain.Entities;

public class AppVersion
{
    public int Id { get; set; }
    public double Version { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public string AppName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}