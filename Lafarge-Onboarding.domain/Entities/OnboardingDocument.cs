namespace Lafarge_Onboarding.domain.Entities;

public class OnboardingDocument
{
    public int Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // Extracted text content
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string UploadedBy { get; set; } = string.Empty; // User ID or name
}