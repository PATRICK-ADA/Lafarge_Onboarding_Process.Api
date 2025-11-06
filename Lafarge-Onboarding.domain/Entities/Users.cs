namespace Lafarge_Onboarding.domain.Entities;

public class Users : IdentityUser
{
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required string Role { get; set; } = UserRoles.LocalHire; // LOCAL_HIRE, EXPAT, VISITOR, HR_ADMIN
    public string? Department { get; set; }
    public string OnboardingStatus { get; set; } = "NotStarted";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}