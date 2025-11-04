namespace Lafarge_Onboarding.domain.Entities;

public class Users : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.LocalHire; // LOCAL_HIRE, EXPAT, VISITOR, HR_ADMIN
    public string? Department { get; set; }
    public string OnboardingStatus { get; set; } = "NotStarted";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}