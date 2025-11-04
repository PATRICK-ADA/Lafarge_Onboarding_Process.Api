namespace Lafarge_Onboarding.domain.OnboardingRequests;

public class DocumentUploadRequest
{
    public string? ContentHeading { get; set; }
    public string? ContentSubHeading { get; set; }
    public IFormFile? ContentBodyUpload { get; set; }
    public IFormFile? ImageUpload { get; set; }
}