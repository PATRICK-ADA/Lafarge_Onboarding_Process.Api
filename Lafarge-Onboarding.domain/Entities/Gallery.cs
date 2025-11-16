namespace Lafarge_Onboarding.domain.Entities;

public class Gallery
{
    public int Id { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
    public string ImageType { get; set; } = string.Empty; // CEO, HR, GENERAL
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? UploadedBy { get; set; }
}