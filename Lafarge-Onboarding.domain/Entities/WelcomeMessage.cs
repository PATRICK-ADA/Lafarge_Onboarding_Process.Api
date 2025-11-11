namespace Lafarge_Onboarding.domain.Entities;

public class WelcomeMessage
{
    public int Id { get; set; }
    public string CeoName { get; set; } = string.Empty;
    public string CeoTitle { get; set; } = string.Empty;
    public string CeoImageUrl { get; set; } = string.Empty;
    public string CeoMessage { get; set; } = string.Empty;
    public string HrName { get; set; } = string.Empty;
    public string HrTitle { get; set; } = string.Empty;
    public string HrImageUrl { get; set; } = string.Empty;
    public string HrMessage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}