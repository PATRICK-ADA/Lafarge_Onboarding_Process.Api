namespace Lafarge_Onboarding.domain.Entities;

public class OnboardingDocument
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public string? Content { get; set; }
    public string? ContentHeading { get; set; }
    public string? ContentSubHeading { get; set; }
    public string? BodyContentFileType { get; set; }
    public string? BodyFilePath { get; set; }
    public string? BodyContent { get; set; }
    public string? ImageFilePath { get; set; }
    public string? ImageFileType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? UploadedBy { get; set; } // User ID or name
}