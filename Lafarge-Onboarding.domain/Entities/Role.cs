namespace Lafarge_Onboarding.domain.Entities;

public class Role : IdentityRole
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}