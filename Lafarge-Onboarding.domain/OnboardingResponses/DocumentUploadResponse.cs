namespace Lafarge_Onboarding.domain.OnboardingResponses;

public class DocumentUploadResponse
{
    public string Message { get; set; } = string.Empty;
    public int DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int ContentLength { get; set; }
    public string Content { get; set; } = string.Empty;
}