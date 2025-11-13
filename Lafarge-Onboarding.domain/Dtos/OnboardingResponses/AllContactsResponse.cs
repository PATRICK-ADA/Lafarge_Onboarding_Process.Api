namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public class AllContactsResponse
{
    public List<EmergencyContact> Emergency { get; set; } = new();
    public List<LafargeContact> Lafarge { get; set; } = new();
    public List<EmbassyContact> Embassies { get; set; } = new();
    public List<HrContact> Hr { get; set; } = new();
}

public class EmergencyContact
{
    public string Service { get; set; } = string.Empty;
    public string Info { get; set; } = string.Empty;
}

public class LafargeContact
{
    public string Function { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class EmbassyContact
{
    public string Embassy { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class HrContact
{
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}