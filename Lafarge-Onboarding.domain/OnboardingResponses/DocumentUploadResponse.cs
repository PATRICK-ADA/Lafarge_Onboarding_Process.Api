namespace Lafarge_Onboarding.domain.OnboardingResponses;

public class DocumentUploadResponse
{
    public string? BodyContentFileType { get; set; } = string.Empty;
    public string? BodyFilePath { get; set; } = string.Empty;
    public string? BodyContent { get; set; } = string.Empty;
    public string? ImageFilePath { get; set; }
    public string? ImageFileType { get; set; }
    public string? ContentHeading { get; set; }
    public string? ContentSubHeading { get; set; }
}