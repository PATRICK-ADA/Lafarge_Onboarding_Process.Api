namespace Lafarge_Onboarding.domain.Dtos.OnboardingRequests;

public sealed record DocumentUploadRequest
{
    public string? ContentHeading { get; init; }
    public string? ContentSubHeading { get; init; }
    public IFormFile? ContentBodyUpload { get; init; }
    public IFormFile? ImageUpload { get; init; }
}