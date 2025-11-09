namespace Lafarge_Onboarding.domain.OnboardingResponses;

public sealed record DocumentUploadResponse
{
    public string? BodyContentFileType { get; init; } = string.Empty;
    public string? BodyFilePath { get; init; } = string.Empty;
    public string? BodyContent { get; init; } = string.Empty;
    public string? ImageFilePath { get; init; }
    public string? ImageFileType { get; init; }
    public string? ContentHeading { get; init; }
    public string? ContentSubHeading { get; init; }
}